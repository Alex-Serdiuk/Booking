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

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        //[Range(0, 5)]
        [Range(0.0, 10.0)]
        public double Rating { get; set; }

        public virtual ICollection<Room> Rooms { get; set;}

        //[Required]
        //public decimal CheapestPrice { get; set; }
        public decimal? CheapestPrice
        {
            get
            {
                return Rooms?.Any() == true ? Rooms.Min(room => room.Price) : null;
            }
        }

        public bool Featured { get; set; }
    }
}
