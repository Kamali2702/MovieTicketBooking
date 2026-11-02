namespace MovieTicketBooking.DTOs.Seats
{
    public class BulkCreateSeatsRequest
    {
        public int ScreenId { get; set; }

        public List<CreateSeatRequest> Seats { get; set; } = new();
    }
}