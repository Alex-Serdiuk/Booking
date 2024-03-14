using System.ComponentModel.DataAnnotations;

namespace BookingServer.Models.Forms
{
    public class HotelFilter
    {
        public int? Max { get; set; }
        public int? Min { get; set; }
        public int? Limit { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? City { get; set; }
       // public string? Address { get; set; }
       // public string? Distance { get; set; }
        //  public string? Title { get; set; }
       // public string? Description { get; set; }

        public bool? Featured { get; set; }
    }
}
