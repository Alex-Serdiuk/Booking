using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

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
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<RoomNumber> RoomNumbers { get; set; }
        public virtual DbSet<UnavailableDate> UnavailableDates { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Hotel>()
                .Property(h => h.CheapestPrice)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Room>()
                .Property(r => r.Price)
                .HasColumnType("decimal(18,2)");


            base.OnModelCreating(builder);
        }
    }
}
