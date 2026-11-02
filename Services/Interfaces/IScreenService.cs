using MovieTicketBooking.DTOs.Screens;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IScreenService
    {
        Task<ScreenResponse> CreateAsync(CreateScreenRequest request);
        Task<List<ScreenResponse>> GetAllAsync();
        Task<ScreenResponse?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, UpdateScreenRequest request);
        Task<bool> DeleteAsync(int id);
    }
}