namespace MovieBuddy.Models
{
    public class UserRating
    {
        public int UserId { get; set; }
        public int MovieId { get; set; } // ID do filme no TheMovieDB
        public bool IsTvShow { get; set; } // Se é série ou filme
        public double Rating { get; set; } // Nota de 0 a 10
        public DateTime RatedAt { get; set; } = DateTime.UtcNow;
        public User User { get; set; }
    }
}
