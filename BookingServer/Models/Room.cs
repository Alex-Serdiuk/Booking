using System.ComponentModel.DataAnnotations;

namespace BookingServer.Models
{
    public class Room
    {
        public Room()
        {
            RoomNumbers = new HashSet<RoomNumber>();
            RoomImages = new HashSet<RoomImage>();
        }

        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int MaxPeople { get; set; }

        [Required]
        public string Description { get; set; }

        public virtual ICollection<RoomNumber> RoomNumbers { get; set; }

        //public DateTime CreatedAt { get; set; }
        //public DateTime UpdatedAt { get; set; }
        public virtual ICollection<RoomImage> RoomImages { get; set; }
        public virtual Hotel Hotel { get; set; }

    }


}
