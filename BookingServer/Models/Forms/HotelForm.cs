using System.ComponentModel.DataAnnotations;

namespace BookingServer.Models.Forms
{
    public class HotelForm
    {
        public HotelForm()
        {
            Photos = new List<string>();
            Rooms = new List<int>();
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Distance { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal CheapestPrice { get; set; }
        public bool Featured { get; set; }

        public List<string>? Photos { get; set; }
        public List<int>? Rooms { get; set; }
    }
}
