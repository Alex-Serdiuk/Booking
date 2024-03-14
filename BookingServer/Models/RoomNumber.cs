namespace BookingServer.Models
{
    public class RoomNumber
    {
        public RoomNumber()
        {
            UnavailableDates = new HashSet<UnavailableDate>(); ;
        }

        public int Id { get; set; }
        public int Number { get; set; }
        public virtual ICollection<UnavailableDate> UnavailableDates { get; set; }
        //public List<DateTime> UnavailableDates { get; set; }
        public virtual Room Room { get; set; }
    }
}
