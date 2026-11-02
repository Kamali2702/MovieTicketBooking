using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IShowRepository
    {
        Task<Show> CreateAsync(Show show);

        Task<List<Show>> GetAllAsync(
            int pageNumber,
            int pageSize,
            DateTime? date,
            int? movieId,
            int? theatreId);

        Task<Show?> GetByIdAsync(int id);

        Task<List<Show>> GetByMovieIdAsync(int movieId);

        Task<List<Show>> GetByTheatreIdAsync(int theatreId);

        Task<List<Show>> GetByDateAsync(DateTime date);

        Task<bool> HasOverlapAsync(
            int screenId,
            DateTime startTime,
            DateTime endTime,
            int? excludeShowId = null);

        Task<List<Seat>> GetSeatsByScreenIdAsync(int screenId);

        Task<List<ShowSeat>> GetShowSeatsByShowIdAsync(int showId);

        Task AddShowSeatsAsync(List<ShowSeat> showSeats);

        Task UpdateAsync(Show show);

        Task DeleteAsync(Show show);

        Task DeleteShowSeatsAsync(List<ShowSeat> showSeats);
    }
}