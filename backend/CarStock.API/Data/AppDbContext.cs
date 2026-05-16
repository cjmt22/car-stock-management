using CarStock.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CarStock.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Car> Cars => Set<Car>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<CarImage> CarImages => Set<CarImage>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Unique email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Store enums as readable strings in the DB
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            // Map IsActive to the existing column name
            modelBuilder.Entity<User>()
                .Property(u => u.IsActive)
                .HasColumnName("isActive");

            modelBuilder.Entity<Car>()
                .Property(c => c.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Booking>()
                .Property(b => b.Status)
                .HasConversion<string>();

            // Dealer → Cars
            modelBuilder.Entity<Car>()
                .HasOne(c => c.Dealer)
                .WithMany(u => u.Cars)
                .HasForeignKey(c => c.DealerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Buyer → Bookings
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Buyer)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Decimal precision for price
            modelBuilder.Entity<Car>()
                .Property(c => c.Price)
                .HasPrecision(18, 2);
        }

    }
}
