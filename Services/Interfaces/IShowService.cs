using MovieTicketBooking.DTOs.Shows;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IShowService
    {
        Task<ShowResponse> CreateAsync(CreateShowRequest request);

        Task<List<ShowResponse>> GetAllAsync(
            int pageNumber,
            int pageSize,
            DateTime? date,
            int? movieId,
            int? theatreId);

        Task<ShowResponse?> GetByIdAsync(int id);

        Task<List<ShowResponse>> GetByMovieIdAsync(int movieId);

        Task<List<ShowResponse>> GetByTheatreIdAsync(int theatreId);

        Task<List<ShowResponse>> GetByDateAsync(DateTime date);

        Task<List<ShowSeatResponse>> GetShowSeatsAsync(int showId);

        Task UpdateAsync(int id, UpdateShowRequest request);

        Task DeleteAsync(int id);
    }
}