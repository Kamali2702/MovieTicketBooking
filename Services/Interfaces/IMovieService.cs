using MovieTicketBooking.DTOs.Movies;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IMovieService
    {
        Task<MovieResponse> CreateAsync(CreateMovieRequest request);

        Task<List<MovieResponse>> GetAllAsync(
            int pageNumber,
            int pageSize,
            string? title);

        Task<MovieResponse?> GetByIdAsync(int id);

        Task<MovieResponse?> SearchByTitleAsync(string title);

        Task<bool> UpdateAsync(
            int id,
            UpdateMovieRequest request);

        Task<bool> DeleteAsync(int id);
    }
}