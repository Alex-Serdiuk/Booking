namespace BookingServer.Models
{
    public class UnavailableDate
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public virtual RoomNumber RoomNumber { get; set; }
        public virtual Booking Booking { get; set; }

    }
}
