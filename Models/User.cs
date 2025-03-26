namespace MovieBuddy.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<UserPreference> Preferences { get; set; }
        public ICollection<UserRating> Ratings { get; set; }
    }
}
