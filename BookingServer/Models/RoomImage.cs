namespace BookingServer.Models
{
    public class RoomImage
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public virtual Room Room { get; set; }
    }
}
