using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class ShowService : IShowService
    {
        private readonly IShowRepository _showRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IScreenRepository _screenRepository;

        public ShowService(
            IShowRepository showRepository,
            IMovieRepository movieRepository,
            IScreenRepository screenRepository)
        {
            _showRepository = showRepository;
            _movieRepository = movieRepository;
            _screenRepository = screenRepository;
        }

        public async Task<ShowResponse> CreateAsync(
            CreateShowRequest request)
        {
            if (request.StartTime >= request.EndTime)
            {
                throw new InvalidOperationException(
                    "End time must be later than start time.");
            }

            if (request.TicketPrice < 0)
            {
                throw new InvalidOperationException(
                    "Ticket price cannot be negative.");
            }

            var movie = await _movieRepository.GetByIdAsync(
                request.MovieId);

            if (movie == null)
            {
                throw new KeyNotFoundException(
                    "Movie not found.");
            }

            var screen = await _screenRepository.GetByIdAsync(
                request.ScreenId);

            if (screen == null)
            {
                throw new KeyNotFoundException(
                    "Screen not found.");
            }

            var hasOverlap = await _showRepository.HasOverlapAsync(
                request.ScreenId,
                request.StartTime,
                request.EndTime);

            if (hasOverlap)
            {
                throw new InvalidOperationException(
                    "Another show is already scheduled during this time on the selected screen.");
            }

            var seats = await _showRepository.GetSeatsByScreenIdAsync(
                request.ScreenId);

            if (seats.Count == 0)
            {
                throw new InvalidOperationException(
                    "No seats are configured for the selected screen.");
            }

            var show = new Show
            {
                MovieId = request.MovieId,
                ScreenId = request.ScreenId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                TicketPrice = request.TicketPrice
            };

            await _showRepository.CreateAsync(show);

            var showSeats = seats.Select(seat => new ShowSeat
            {
                ShowId = show.ShowId,
                SeatId = seat.SeatId,
                Status = "Available"
            }).ToList();

            await _showRepository.AddShowSeatsAsync(showSeats);

            return MapToResponse(show);
        }

        public async Task<List<ShowResponse>> GetAllAsync(
            int pageNumber,
            int pageSize,
            DateTime? date,
            int? movieId,
            int? theatreId)
        {
            if (pageNumber < 1)
            {
                throw new ArgumentException(
                    "Page number must be greater than 0.");
            }

            if (pageSize < 1)
            {
                throw new ArgumentException(
                    "Page size must be greater than 0.");
            }

            if (pageSize > 100)
            {
                throw new ArgumentException(
                    "Page size cannot be greater than 100.");
            }

            var shows = await _showRepository.GetAllAsync(
                pageNumber,
                pageSize,
                date,
                movieId,
                theatreId);

            return shows
                .Select(MapToResponse)
                .ToList();
        }

        public async Task<ShowResponse?> GetByIdAsync(int id)
        {
            var show = await _showRepository.GetByIdAsync(id);

            if (show == null)
            {
                return null;
            }

            return MapToResponse(show);
        }

        public async Task<List<ShowResponse>> GetByMovieIdAsync(
            int movieId)
        {
            var shows = await _showRepository.GetByMovieIdAsync(
                movieId);

            return shows
                .Select(MapToResponse)
                .ToList();
        }

        public async Task<List<ShowResponse>> GetByTheatreIdAsync(
            int theatreId)
        {
            var shows = await _showRepository.GetByTheatreIdAsync(
                theatreId);

            return shows
                .Select(MapToResponse)
                .ToList();
        }

        public async Task<List<ShowResponse>> GetByDateAsync(
            DateTime date)
        {
            var shows = await _showRepository.GetByDateAsync(date);

            return shows
                .Select(MapToResponse)
                .ToList();
        }

        public async Task<List<ShowSeatResponse>> GetShowSeatsAsync(
            int showId)
        {
            var show = await _showRepository.GetByIdAsync(showId);

            if (show == null)
            {
                throw new KeyNotFoundException(
                    "Show not found.");
            }

            var seats = await _showRepository.GetSeatsByScreenIdAsync(
                show.ScreenId);

            var showSeats =
                await _showRepository.GetShowSeatsByShowIdAsync(
                    showId);

            var showSeatStatus = showSeats.ToDictionary(
                ss => ss.SeatId,
                ss => ss.Status);

            return seats.Select(seat => new ShowSeatResponse
            {
                SeatId = seat.SeatId,
                RowName = seat.RowName,
                SeatNumber = seat.SeatNumber,
                SeatLabel = $"{seat.RowName}{seat.SeatNumber}",
                SeatType = seat.SeatType,
                Price = show.TicketPrice,
                Status = showSeatStatus.TryGetValue(
                    seat.SeatId,
                    out var status)
                    ? status
                    : "Available"
            }).ToList();
        }

        public async Task UpdateAsync(
            int id,
            UpdateShowRequest request)
        {
            if (request.StartTime >= request.EndTime)
            {
                throw new InvalidOperationException(
                    "End time must be later than start time.");
            }

            if (request.TicketPrice < 0)
            {
                throw new InvalidOperationException(
                    "Ticket price cannot be negative.");
            }

            var show = await _showRepository.GetByIdAsync(id);

            if (show == null)
            {
                throw new KeyNotFoundException(
                    "Show not found.");
            }

            var movie = await _movieRepository.GetByIdAsync(
                request.MovieId);

            if (movie == null)
            {
                throw new KeyNotFoundException(
                    "Movie not found.");
            }

            var screen = await _screenRepository.GetByIdAsync(
                request.ScreenId);

            if (screen == null)
            {
                throw new KeyNotFoundException(
                    "Screen not found.");
            }

            var hasOverlap = await _showRepository.HasOverlapAsync(
                request.ScreenId,
                request.StartTime,
                request.EndTime,
                id);

            if (hasOverlap)
            {
                throw new InvalidOperationException(
                    "Another show is already scheduled during this time on the selected screen.");
            }

            show.MovieId = request.MovieId;
            show.ScreenId = request.ScreenId;
            show.StartTime = request.StartTime;
            show.EndTime = request.EndTime;
            show.TicketPrice = request.TicketPrice;

            await _showRepository.UpdateAsync(show);
        }

        public async Task DeleteAsync(int id)
        {
            var show = await _showRepository.GetByIdAsync(id);

            if (show == null)
            {
                throw new KeyNotFoundException(
                    "Show not found.");
            }

            var showSeats =
                await _showRepository.GetShowSeatsByShowIdAsync(id);

            if (showSeats.Count > 0)
            {
                await _showRepository.DeleteShowSeatsAsync(
                    showSeats);
            }

            await _showRepository.DeleteAsync(show);
        }

        private static ShowResponse MapToResponse(Show show)
        {
            return new ShowResponse
            {
                ShowId = show.ShowId,
                MovieId = show.MovieId,
                ScreenId = show.ScreenId,
                StartTime = show.StartTime,
                EndTime = show.EndTime,
                TicketPrice = show.TicketPrice
            };
        }
    }
}