namespace MovieBuddy.DTOs.External
{
    public class TheMovieDbSearchResponse
    {
        public int Page { get; set; }
        public List<TheMovieDbSearchResult> Results { get; set; }
        public int TotalPages { get; set; }
        public int TotalResults { get; set; }
    }

    public class TheMovieDbSearchResult
    {
        public int Id { get; set; }
        public string MediaType { get; set; } // "movie" or "tv"
        public string Title { get; set; } // For movies
        public string Name { get; set; } // For TV shows
        public string Overview { get; set; }
        public string ReleaseDate { get; set; } // For movies
        public string FirstAirDate { get; set; } // For TV shows
        public List<int> GenreIds { get; set; }
        public double VoteAverage { get; set; }
        public int VoteCount { get; set; }
        public double Popularity { get; set; }
        public string PosterPath { get; set; }
        public string BackdropPath { get; set; }
    }
}
