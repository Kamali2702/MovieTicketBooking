using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        Task<Movie> AddAsync(Movie movie);

        Task<List<Movie>> GetAllAsync();

        Task<Movie?> GetByIdAsync(int id);

        Task<Movie?> GetByTitleAsync(string title);

        Task UpdateAsync(Movie movie);

        Task DeleteAsync(Movie movie);
    }
}