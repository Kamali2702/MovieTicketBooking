using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IScreenRepository
    {
        Task<Screen> CreateAsync(Screen screen);

        Task<List<Screen>> GetAllAsync();

        Task<List<Screen>> GetByTheatreIdAsync(int theatreId);

        Task<Screen?> GetByIdAsync(int id);

        Task UpdateAsync(Screen screen);

        Task DeleteAsync(Screen screen);

        Task<bool> ExistsAsync(
            int theatreId,
            string screenName);
    }
}