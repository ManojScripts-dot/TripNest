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

        public DbSet<Review> Reviews { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        


        public DbSet<Agency> Agencies { get; set; }



        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Booking>()
        .Property(b => b.TotalAmount)
        .HasColumnType("numeric(18,2)");
}

    }
}
