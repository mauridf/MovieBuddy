namespace MovieBuddy.Models
{
    public class UserPreference
    {
        public int UserId { get; set; }
        public int GenreId { get; set; } // ID do gênero no TheMovieDB
        public User User { get; set; }
    }
}
