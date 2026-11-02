using MovieTicketBooking.DTOs.Seats;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface ISeatService
    {
        Task<SeatResponse> CreateAsync(
            CreateSeatRequest request);

        Task<List<SeatResponse>> GetByScreenIdAsync(
            int screenId);

        Task<SeatResponse?> GetByIdAsync(int id);

        Task UpdateAsync(
            int id,
            UpdateSeatRequest request);

        Task DeleteAsync(int id);

        Task<List<SeatResponse>> BulkCreateAsync(
            BulkCreateSeatsRequest request);
    }
}