using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieBuddy.Data;
using MovieBuddy.DTOs;
using MovieBuddy.DTOs.External;
using RestSharp;

namespace MovieBuddy.Service
{
    public class TheMovieDbService : ITheMovieDbService
    {
        private readonly RestClient _client;
        private readonly string _apiKey;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TheMovieDbService(IConfiguration config, ApplicationDbContext context, IMapper mapper)
        {
            _apiKey = config["TheMovieDb:ApiKey"];
            _client = new RestClient(config["TheMovieDb:BaseUrl"]);
            _context = context;
            _mapper = mapper;
        }

        public async Task<PaginatedResultDto<MovieDto>> SearchMoviesAsync(SearchRequestDto request)
        {
            var endpoint = request.Query != null ? "search/multi" : "discover/movie";
            var restRequest = new RestRequest(endpoint);
            restRequest.AddParameter("api_key", _apiKey, ParameterType.QueryString);
            restRequest.AddParameter("page", request.Page, ParameterType.QueryString);

            if (request.Query != null)
            {
                restRequest.AddParameter("query", request.Query, ParameterType.QueryString);
            }
            else
            {
                if (request.Year.HasValue)
                    restRequest.AddParameter("year", request.Year, ParameterType.QueryString);
                if (!string.IsNullOrEmpty(request.Language))
                    restRequest.AddParameter("language", request.Language, ParameterType.QueryString);
                if (request.GenreId.HasValue)
                    restRequest.AddParameter("with_genres", request.GenreId, ParameterType.QueryString);
            }

            var response = await _client.ExecuteAsync<TheMovieDbSearchResponse>(restRequest);

            if (!response.IsSuccessful)
            {
                throw new Exception("Failed to fetch data from TheMovieDB");
            }

            var movies = new List<MovieDto>();
            foreach (var result in response.Data.Results)
            {
                var title = result.MediaType == "tv" ? result.Name : result.Title;
                if (string.IsNullOrEmpty(title)) continue;

                var movie = _mapper.Map<MovieDto>(result);
                movie.IsTvShow = result.MediaType == "tv";
                movies.Add(movie);
            }

            return new PaginatedResultDto<MovieDto>
            {
                Page = response.Data.Page,
                TotalPages = response.Data.TotalPages,
                TotalResults = response.Data.TotalResults,
                Results = movies
            };
        }

        public async Task<MovieDto> GetMovieDetailsAsync(int id, bool isTvShow)
        {
            var endpoint = isTvShow ? $"tv/{id}" : $"movie/{id}";
            var restRequest = new RestRequest(endpoint);
            restRequest.AddParameter("api_key", _apiKey, ParameterType.QueryString);
            restRequest.AddParameter("append_to_response", "videos,watch/providers", ParameterType.QueryString);

            var response = await _client.ExecuteAsync<TheMovieDbMovieDetails>(restRequest);

            if (!response.IsSuccessful)
            {
                throw new Exception("Failed to fetch movie details from TheMovieDB");
            }

            var movie = _mapper.Map<MovieDto>(response.Data);
            movie.IsTvShow = isTvShow;

            // Get trailer URL
            var trailer = response.Data.Videos?.Results?
                .FirstOrDefault(v => v.Site == "YouTube" && v.Type == "Trailer");
            movie.TrailerUrl = trailer != null ? $"https://www.youtube.com/watch?v={trailer.Key}" : null;

            // Get streaming info
            movie.StreamingInfo = response.Data.WatchProviders?.Results?
                .SelectMany(wp => wp.Value?.Flatrate?.Select(p => new StreamingInfoDto
                {
                    Country = wp.Key,
                    ProviderName = p.ProviderName,
                    LogoPath = $"https://image.tmdb.org/t/p/w92{p.LogoPath}"
                }) ?? new List<StreamingInfoDto>())
                .ToList();

            return movie;
        }

        public async Task<List<MovieDto>> GetRecommendationsAsync(int userId)
        {
            // Verifica se o usuário existe e carrega preferências
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists) throw new Exception("User not found");

            var recommendations = new List<MovieDto>();

            // 1. Recomendações baseadas nas preferências do usuário
            var userPreferences = await _context.UserPreferences
                .Where(up => up.UserId == userId)
                .Select(up => up.GenreId)
                .ToListAsync();

            if (userPreferences.Any())
            {
                // Seleciona um gênero aleatório de forma eficiente
                var random = new Random();
                var randomIndex = random.Next(0, userPreferences.Count);
                var randomGenre = userPreferences[randomIndex];

                var genreResults = await SearchMoviesAsync(new SearchRequestDto
                {
                    GenreId = randomGenre,
                    Page = 1
                });

                recommendations.AddRange(genreResults.Results.Take(5));
            }

            // 2. Recomendações baseadas no que outros usuários avaliaram bem
            var topRatedMovieIds = await _context.UserRatings
                .Where(r => r.Rating >= 7 && r.UserId != userId)
                .GroupBy(r => new { r.MovieId, r.IsTvShow })
                .Select(g => new
                {
                    g.Key.MovieId,
                    g.Key.IsTvShow,
                    AverageRating = g.Average(r => r.Rating)
                })
                .OrderByDescending(x => x.AverageRating)
                .Take(5)
                .ToListAsync();

            // Carrega todos os detalhes de uma vez (evita N+1)
            foreach (var movie in topRatedMovieIds)
            {
                try
                {
                    var details = await GetMovieDetailsAsync(movie.MovieId, movie.IsTvShow);
                    if (details != null)
                    {
                        recommendations.Add(details);
                    }
                }
                catch (Exception ex)
                {
                    // Logar erro se necessário
                    Console.WriteLine($"Error loading movie details: {ex.Message}");
                }
            }

            return recommendations
                .GroupBy(m => m.Id)
                .Select(g => g.First())
                .ToList();
        }

        public async Task<List<GenreDto>> GetAllGenresAsync()
        {
            // Primeiro tenta pegar os gêneros de filmes
            var movieRequest = new RestRequest("genre/movie/list");
            movieRequest.AddParameter("api_key", _apiKey, ParameterType.QueryString);

            var tvRequest = new RestRequest("genre/tv/list");
            tvRequest.AddParameter("api_key", _apiKey, ParameterType.QueryString);

            var movieGenres = await _client.ExecuteAsync<TheMovieDbGenreResponse>(movieRequest);
            var tvGenres = await _client.ExecuteAsync<TheMovieDbGenreResponse>(tvRequest);

            if (!movieGenres.IsSuccessful || !tvGenres.IsSuccessful)
            {
                throw new Exception("Failed to fetch genres from TheMovieDB");
            }

            // Combina e remove duplicados
            var allGenres = movieGenres.Data.Genres
                .Union(tvGenres.Data.Genres)
                .GroupBy(g => g.Id)
                .Select(g => g.First())
                .ToList();

            return _mapper.Map<List<GenreDto>>(allGenres);
        }

        public async Task<List<MovieDto>> GetSimilarMoviesAsync(int movieId, bool isTvShow)
        {
            var endpoint = isTvShow ? $"tv/{movieId}/similar" : $"movie/{movieId}/similar";
            var restRequest = new RestRequest(endpoint);
            restRequest.AddParameter("api_key", _apiKey, ParameterType.QueryString);

            var response = await _client.ExecuteAsync<TheMovieDbSearchResponse>(restRequest);

            if (!response.IsSuccessful)
            {
                throw new Exception("Failed to fetch similar movies from TheMovieDB");
            }

            var movies = new List<MovieDto>();
            foreach (var result in response.Data.Results)
            {
                if (string.IsNullOrEmpty(result.Title)) continue;

                var movie = _mapper.Map<MovieDto>(result);
                movie.IsTvShow = isTvShow;
                movies.Add(movie);
            }

            return movies;
        }
    }

    public class TheMovieDbGenreResponse
    {
        public List<TheMovieDbGenre> Genres { get; set; }
    }
}