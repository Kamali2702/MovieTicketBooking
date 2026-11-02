namespace MovieTicketBooking.DTOs.Bookings
{
    public class CreateBookingRequest
    {
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public int ShowId { get; set; }
        public List<int> SeatIds { get; set; } = new();
    }
}