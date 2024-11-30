using DriveX_Backend.Entities.Cars;
using DriveX_Backend.Entities.RentalRequest;
using DriveX_Backend.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace DriveX_Backend.DB
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Brand> Brands { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<CarImage> CarImages { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<PhoneNumber> PhoneNumbers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RentalRequest> RentalRequests { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //brand-model binding
            modelBuilder.Entity<Model>()
                .HasOne(m => m.Brand)
                .WithMany(b => b.Models)
                .HasForeignKey(m => m.BrandId)
                .OnDelete(DeleteBehavior.Cascade);

            //car-brand realationship
            modelBuilder.Entity<Car>()
                .HasOne(c => c.Brand)
                .WithMany()
                .HasForeignKey(c => c.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            // Car - Model relationship
            modelBuilder.Entity<Car>()
                .HasOne(c => c.Model)
                .WithMany()
                .HasForeignKey(c => c.ModelId)
                .OnDelete(DeleteBehavior.Restrict);


            // Car - CarImage relationship
            modelBuilder.Entity<CarImage>()
                .HasOne(c => c.Car)
                .WithMany(c => c.Images)
                .HasForeignKey(ci => ci.CarId)
                .OnDelete(DeleteBehavior.Cascade);


            // User - Address relationship
            modelBuilder.Entity<Address>()
                .HasOne<User>()
                .WithMany(u => u.Addresses)
                .HasForeignKey(a =>a.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // User - PhoneNumber relationship
            modelBuilder.Entity<PhoneNumber>()
               .HasOne<User>()
               .WithMany(u => u.PhoneNumbers)
               .HasForeignKey(u =>u.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            //RentalRequest - Car Relationship
            modelBuilder.Entity<RentalRequest>()
                .HasOne(rr=>rr.Car)
                .WithMany()
                .HasForeignKey(rr=>rr.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            //RentalRequest - User Relationship

            modelBuilder.Entity<RentalRequest>()
                .HasOne(rr=>rr.User)
                .WithMany()
                .HasForeignKey(rr=>rr.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Set precision and scale for OngoingRevenue and TotalRevenue
            modelBuilder.Entity<Car>()
                .Property(c => c.OngoingRevenue)
                .HasColumnType("decimal(18,2)");  // Precision 18, Scale 2

            modelBuilder.Entity<Car>()
                .Property(c => c.TotalRevenue)
                .HasColumnType("decimal(18,2)");  // Precision 18, Scale 2
        }

    }


}
