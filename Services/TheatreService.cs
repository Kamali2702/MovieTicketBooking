using MovieTicketBooking.DTOs.Theatres;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class TheatreService : ITheatreService
    {
        private readonly ITheatreRepository _theatreRepository;

        public TheatreService(ITheatreRepository theatreRepository)
        {
            _theatreRepository = theatreRepository;
        }

        public async Task<TheatreResponse> CreateAsync(CreateTheatreRequest request)
        {
            var theatre = new Theatre
            {
                Name = request.Name,
                Location = request.Location
            };

            var createdTheatre = await _theatreRepository.AddAsync(theatre);

            return MapToResponse(createdTheatre);
        }

        public async Task<List<TheatreResponse>> GetAllAsync()
        {
            var theatres = await _theatreRepository.GetAllAsync();

            return theatres.Select(MapToResponse).ToList();
        }

        public async Task<TheatreResponse?> GetByIdAsync(int id)
        {
            var theatre = await _theatreRepository.GetByIdAsync(id);

            return theatre == null ? null : MapToResponse(theatre);
        }

        public async Task<bool> UpdateAsync(
            int id,
            UpdateTheatreRequest request)
        {
            var theatre = await _theatreRepository.GetByIdAsync(id);

            if (theatre == null)
                return false;

            theatre.Name = request.Name;
            theatre.Location = request.Location;

            await _theatreRepository.UpdateAsync(theatre);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var theatre = await _theatreRepository.GetByIdAsync(id);

            if (theatre == null)
                return false;

            await _theatreRepository.DeleteAsync(theatre);

            return true;
        }

        private static TheatreResponse MapToResponse(Theatre theatre)
        {
            return new TheatreResponse
            {
                TheatreId = theatre.TheatreId,
                Name = theatre.Name,
                Location = theatre.Location
            };
        }
    }
}