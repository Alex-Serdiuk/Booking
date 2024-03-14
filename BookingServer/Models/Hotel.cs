using System.ComponentModel.DataAnnotations;

namespace BookingServer.Models
{
    public class Hotel
    {
        public Hotel()
        {
            HotelImages = new HashSet<HotelImage>();
            Rooms = new HashSet<Room>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Distance { get; set; }

        public virtual ICollection<HotelImage> HotelImages { get; set; }
        //public List<string> Photos { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Range(0, 5)]
        public int Rating { get; set; }

        public virtual ICollection<Room> Rooms { get; set;}
        //public List<string> Rooms { get; set; }

        [Required]
        public decimal CheapestPrice { get; set; }

        public bool Featured { get; set; }
    }
}
