using MovieTicketBooking.DTOs.Theatres;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface ITheatreService
    {
        Task<TheatreResponse> CreateAsync(CreateTheatreRequest request);
        Task<List<TheatreResponse>> GetAllAsync();
        Task<TheatreResponse?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, UpdateTheatreRequest request);
        Task<bool> DeleteAsync(int id);
    }
}