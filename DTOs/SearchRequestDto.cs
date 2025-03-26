namespace MovieBuddy.DTOs
{
    public class SearchRequestDto
    {
        public string Query { get; set; }
        public int? Year { get; set; }
        public string Language { get; set; } = "pt-BR"; // Default para português
        public int? GenreId { get; set; }
        public int Page { get; set; } = 1;
        public bool IncludeAdult { get; set; } = false;
    }
}
