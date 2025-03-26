namespace MovieBuddy.DTOs
{
    public class MovieDto
    {
        public int Id { get; set; }
        public bool IsTvShow { get; set; }
        public string? Title { get; set; }
        public string? Overview { get; set; }
        public string? ReleaseDate { get; set; }
        public int? Runtime { get; set; }
        public List<string>? Genres { get; set; }
        public double VoteAverage { get; set; }
        public int VoteCount { get; set; }
        public double Popularity { get; set; }
        public string? PosterPath { get; set; }
        public string? TrailerUrl { get; set; }
        public List<StreamingInfoDto>? StreamingInfo { get; set; }
    }
}
