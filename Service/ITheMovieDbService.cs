using MovieBuddy.DTOs;

namespace MovieBuddy.Service
{
    public interface ITheMovieDbService
    {
        Task<PaginatedResultDto<MovieDto>> SearchMoviesAsync(SearchRequestDto request);
        Task<MovieDto> GetMovieDetailsAsync(int id, bool isTvShow);
        Task<List<MovieDto>> GetRecommendationsAsync(int userId);
        Task<List<GenreDto>> GetAllGenresAsync();
        Task<List<MovieDto>> GetSimilarMoviesAsync(int movieId, bool isTvShow);
    }
}
