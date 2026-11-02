using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IScreenRepository
    {
        Task<Screen> AddAsync(Screen screen);
        Task<List<Screen>> GetAllAsync();
        Task<Screen?> GetByIdAsync(int id);
        Task UpdateAsync(Screen screen);
        Task DeleteAsync(Screen screen);
    }
}