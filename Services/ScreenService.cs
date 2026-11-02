using MovieTicketBooking.DTOs.Screens;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class ScreenService : IScreenService
    {
        private readonly IScreenRepository _screenRepository;

        public ScreenService(IScreenRepository screenRepository)
        {
            _screenRepository = screenRepository;
        }

        public async Task<ScreenResponse> CreateAsync(CreateScreenRequest request)
        {
            var screen = new Screen
            {
                TheatreId = request.TheatreId,
                ScreenName = request.ScreenName
            };

            var createdScreen = await _screenRepository.AddAsync(screen);

            return MapToResponse(createdScreen);
        }

        public async Task<List<ScreenResponse>> GetAllAsync()
        {
            var screens = await _screenRepository.GetAllAsync();

            return screens.Select(MapToResponse).ToList();
        }

        public async Task<ScreenResponse?> GetByIdAsync(int id)
        {
            var screen = await _screenRepository.GetByIdAsync(id);

            return screen == null ? null : MapToResponse(screen);
        }

        public async Task<bool> UpdateAsync(int id, UpdateScreenRequest request)
        {
            var screen = await _screenRepository.GetByIdAsync(id);

            if (screen == null)
                return false;

            screen.ScreenName = request.ScreenName;

            await _screenRepository.UpdateAsync(screen);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var screen = await _screenRepository.GetByIdAsync(id);

            if (screen == null)
                return false;

            await _screenRepository.DeleteAsync(screen);

            return true;
        }

        private static ScreenResponse MapToResponse(Screen screen)
        {
            return new ScreenResponse
            {
                ScreenId = screen.ScreenId,
                TheatreId = screen.TheatreId,
                ScreenName = screen.ScreenName
            };
        }
    }
}