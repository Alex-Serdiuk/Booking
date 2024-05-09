namespace BookingServer.Models.Forms
{
    public class RoomForm
    {
        public RoomForm()
        {
            RoomNumbers = new List<RoomNumberForm>();
            Photos = new List<string>();
        }

        public string Title { get; set; }

        public decimal Price { get; set; }

        public int MaxPeople { get; set; }

        public string Description { get; set; }

        
        public List<RoomNumberForm>? RoomNumbers { get; set; }
        public List<string>? Photos { get; set; }
    }
}
