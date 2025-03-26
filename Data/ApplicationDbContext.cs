using Microsoft.EntityFrameworkCore;
using MovieBuddy.Models;

namespace MovieBuddy.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }
        public DbSet<UserRating> UserRatings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserPreference>()
                .HasKey(up => new { up.UserId, up.GenreId });

            modelBuilder.Entity<UserRating>()
                .HasKey(ur => new { ur.UserId, ur.MovieId });
        }
    }
}
