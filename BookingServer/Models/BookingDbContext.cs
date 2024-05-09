using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Microsoft.Extensions.DependencyInjection;

namespace BookingServer.Models
{
    public class BookingDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public BookingDbContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Hotel> Hotels { get; set; }
        public virtual DbSet<HotelImage> HotelImages { get; set; }
        public virtual DbSet<RoomImage> RoomImages { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<RoomNumber> RoomNumbers { get; set; }
        public virtual DbSet<UnavailableDate> UnavailableDates { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.Entity<Hotel>()
            //    .Property(h => h.CheapestPrice)
            //    .HasColumnType("decimal(18,2)");

            builder.Entity<Room>()
                .Property(r => r.Price)
                .HasColumnType("decimal(18,2)");


            base.OnModelCreating(builder);

            //// Створення адміністратора при першому запуску
            //var adminUser = new User
            //{
            //    UserName = "admin",
            //    Email = "admin@example.com",
            //    // Додайте інші необхідні вам властивості користувача
            //};

            //var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            //userManager.CreateAsync(adminUser, "12345").GetAwaiter().GetResult();

            //// Додайте роль "Admin" користувачеві
            //userManager.AddToRoleAsync(adminUser, "Admin").GetAwaiter().GetResult();
        }
    }
}
