namespace MovieBuddy.DTOs
{
    public class RatingDto
    {
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public bool IsTvShow { get; set; }
        public double Rating { get; set; } // Ex: 0 a 10
    }
}
