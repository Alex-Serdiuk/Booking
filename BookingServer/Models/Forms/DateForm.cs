namespace BookingServer.Models.Forms
{
    public class DateForm
    {
        public DateForm()
        {
            Dates = new List<DateTime>();
        }

        public List<DateTime> Dates { get; set; }
    }
}
