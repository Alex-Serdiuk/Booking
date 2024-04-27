namespace BookingServer.Models.Forms
{
    public class BookingRequest
    {
        public int UserId { get; set; }
        public List<int> RoomIds { get; set; }
        public List<DateTime> Dates { get; set; }
    }
}
