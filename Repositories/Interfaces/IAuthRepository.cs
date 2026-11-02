using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetByUsernameAsync(string username);

        Task<User?> GetByEmailAsync(string email);

        Task<User> CreateAsync(User user);
    }
}