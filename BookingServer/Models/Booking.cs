namespace BookingServer.Models
{
    public class Booking
    {
        public Booking()
        {
            UnavailableDates = new HashSet<UnavailableDate>(); ;
        }

        public int Id { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<UnavailableDate> UnavailableDates { get; set; }
    }
}
