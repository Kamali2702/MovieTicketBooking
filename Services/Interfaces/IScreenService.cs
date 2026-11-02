using MovieTicketBooking.DTOs.Screens;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IScreenService
    {
        Task<ScreenResponse> CreateAsync(CreateScreenRequest request);

        Task<List<ScreenResponse>> GetAllAsync();

        Task<List<ScreenResponse>> GetByTheatreIdAsync(int theatreId);

        Task<ScreenResponse?> GetByIdAsync(int id);

        Task UpdateAsync(int id, UpdateScreenRequest request);

        Task DeleteAsync(int id);
    }
}