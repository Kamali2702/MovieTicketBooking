using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface ISeatRepository
    {
        Task<Seat> CreateAsync(Seat seat);

        Task<List<Seat>> GetByScreenIdAsync(int screenId);

        Task<Seat?> GetByIdAsync(int id);

        Task UpdateAsync(Seat seat);

        Task DeleteAsync(Seat seat);

        Task<bool> ExistsAsync(
            int screenId,
            string rowName,
            int seatNumber);
    }
}