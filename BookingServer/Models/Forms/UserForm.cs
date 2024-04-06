namespace BookingServer.Models.Forms
{
    public class UserForm
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
        public string Country { get; set; }
        public string? Img { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
    }
}
