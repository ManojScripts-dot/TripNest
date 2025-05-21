using Microsoft.EntityFrameworkCore;
using TripNest.Models;

namespace TripNest.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Tour> Tours { get; set; } = default!;
    }
}
