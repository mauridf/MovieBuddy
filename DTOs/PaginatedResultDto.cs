namespace MovieBuddy.DTOs
{
    public class PaginatedResultDto<T>
    {
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int TotalResults { get; set; }
        public List<T>? Results { get; set; }
    }
}
