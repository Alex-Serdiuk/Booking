using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookingServer.Models
{
    public class User: IdentityUser<int>
    {
        public User()
        {
            Bookings = new HashSet<Booking>();
        }

        //public int Id { get; set; }

        //[Required]
        //[MaxLength(255)]
        //public string Username { get; set; }

        //[Required]
        //[EmailAddress]
        //[MaxLength(255)]
        //public string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string Country { get; set; }

        [MaxLength(255)]
        public string? Img { get; set; }

        [Required]
        [MaxLength(255)]
        public string City { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }

        //[Required]
        //[Phone]
        //[MaxLength(20)]
        //public string Phone { get; set; }

        //[Required]
        //[MaxLength(255)]
        //public string Password { get; set; }

        //public bool IsAdmin { get; set; }

        //public DateTime CreatedAt { get; set; }
        //public DateTime UpdatedAt { get; set; }
    }
}
