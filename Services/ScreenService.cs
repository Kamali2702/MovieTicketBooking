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

        public async Task<ScreenResponse> CreateAsync(
            CreateScreenRequest request)
        {
            if (request.TheatreId <= 0)
                throw new ArgumentException(
                    "Theatre ID must be greater than 0.");

            if (string.IsNullOrWhiteSpace(request.ScreenName))
                throw new ArgumentException(
                    "Screen name is required.");

            var exists = await _screenRepository.ExistsAsync(
                request.TheatreId,
                request.ScreenName);

            if (exists)
                throw new InvalidOperationException(
                    "A screen with the same name already exists for this theatre.");

            var screen = new Screen
            {
                TheatreId = request.TheatreId,
                ScreenName = request.ScreenName
            };

            var createdScreen =
                await _screenRepository.CreateAsync(screen);

            return MapToResponse(createdScreen);
        }

        public async Task<List<ScreenResponse>> GetAllAsync()
        {
            var screens = await _screenRepository.GetAllAsync();

            return screens
                .Select(MapToResponse)
                .ToList();
        }

        public async Task<List<ScreenResponse>> GetByTheatreIdAsync(
            int theatreId)
        {
            var screens =
                await _screenRepository.GetByTheatreIdAsync(theatreId);

            return screens
                .Select(MapToResponse)
                .ToList();
        }

        public async Task<ScreenResponse?> GetByIdAsync(int id)
        {
            var screen = await _screenRepository.GetByIdAsync(id);

            if (screen == null)
                return null;

            return MapToResponse(screen);
        }

        public async Task UpdateAsync(
            int id,
            UpdateScreenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ScreenName))
                throw new ArgumentException(
                    "Screen name is required.");

            var screen = await _screenRepository.GetByIdAsync(id);

            if (screen == null)
                throw new KeyNotFoundException(
                    "Screen not found.");

            var duplicate = await _screenRepository.ExistsAsync(
                screen.TheatreId,
                request.ScreenName);

            if (duplicate &&
                screen.ScreenName != request.ScreenName)
            {
                throw new InvalidOperationException(
                    "A screen with the same name already exists for this theatre.");
            }

            screen.ScreenName = request.ScreenName;

            await _screenRepository.UpdateAsync(screen);
        }

        public async Task DeleteAsync(int id)
        {
            var screen = await _screenRepository.GetByIdAsync(id);

            if (screen == null)
                throw new KeyNotFoundException(
                    "Screen not found.");

            await _screenRepository.DeleteAsync(screen);
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