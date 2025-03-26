namespace MovieBuddy.DTOs
{
    public class UserCreateDto
    {
        public string Name { get; set; }
        public List<int> GenreIds { get; set; } // IDs dos gêneros preferidos
    }
}
