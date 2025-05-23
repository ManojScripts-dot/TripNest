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
        public DbSet<Booking> Bookings { get; set; } = default!;

        public DbSet<UserProfile> UserProfiles { get; set; }

        public DbSet<Feedback> Feedbacks { get; set; }

        public DbSet<Agency> Agencies { get; set; }



        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalAmount)
                .HasColumnType("decimal(18,2)");  // specify precision and scale for TotalAmount

            // Add other model configurations here if needed
        }
    }
}
