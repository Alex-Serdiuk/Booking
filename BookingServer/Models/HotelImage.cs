namespace BookingServer.Models
{
    public class HotelImage
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public virtual Hotel Hotel { get; set; }
    }
}
