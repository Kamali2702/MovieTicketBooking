namespace MovieTicketBooking.DTOs.Seats
{
    public class UpdateSeatRequest
    {
        public string RowName { get; set; } = string.Empty;

        public int SeatNumber { get; set; }

        public string SeatType { get; set; } = string.Empty;
    }
}