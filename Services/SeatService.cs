using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;

        public SeatService(ISeatRepository seatRepository)
        {
            _seatRepository = seatRepository;
        }

        public async Task<SeatResponse> CreateAsync(
            CreateSeatRequest request)
        {
            ValidateSeatRequest(request);

            var exists = await _seatRepository.ExistsAsync(
                request.ScreenId,
                request.RowName,
                request.SeatNumber);

            if (exists)
            {
                throw new InvalidOperationException(
                    "A seat with the same row and seat number already exists for this screen.");
            }

            var seat = new Seat
            {
                ScreenId = request.ScreenId,
                RowName = request.RowName,
                SeatNumber = request.SeatNumber,
                SeatType = request.SeatType
            };

            var createdSeat =
                await _seatRepository.CreateAsync(seat);

            return MapToResponse(createdSeat);
        }

        public async Task<List<SeatResponse>> GetByScreenIdAsync(
            int screenId)
        {
            var seats =
                await _seatRepository.GetByScreenIdAsync(screenId);

            return seats
                .Select(MapToResponse)
                .ToList();
        }

        public async Task<SeatResponse?> GetByIdAsync(int id)
        {
            var seat = await _seatRepository.GetByIdAsync(id);

            if (seat == null)
                return null;

            return MapToResponse(seat);
        }

        public async Task UpdateAsync(
            int id,
            UpdateSeatRequest request)
        {
            if (request.SeatNumber <= 0)
                throw new ArgumentException(
                    "Seat number must be greater than 0.");

            if (string.IsNullOrWhiteSpace(request.RowName))
                throw new ArgumentException(
                    "Row name is required.");

            if (string.IsNullOrWhiteSpace(request.SeatType))
                throw new ArgumentException(
                    "Seat type is required.");

            var seat =
                await _seatRepository.GetByIdAsync(id);

            if (seat == null)
                throw new KeyNotFoundException(
                    "Seat not found.");

            var duplicate =
                await _seatRepository.ExistsAsync(
                    seat.ScreenId,
                    request.RowName,
                    request.SeatNumber);

            if (duplicate &&
                (seat.RowName != request.RowName ||
                 seat.SeatNumber != request.SeatNumber))
            {
                throw new InvalidOperationException(
                    "A seat with the same row and seat number already exists for this screen.");
            }

            seat.RowName = request.RowName;
            seat.SeatNumber = request.SeatNumber;
            seat.SeatType = request.SeatType;

            await _seatRepository.UpdateAsync(seat);
        }

        public async Task DeleteAsync(int id)
        {
            var seat =
                await _seatRepository.GetByIdAsync(id);

            if (seat == null)
                throw new KeyNotFoundException(
                    "Seat not found.");

            await _seatRepository.DeleteAsync(seat);
        }

        public async Task<List<SeatResponse>> BulkCreateAsync(
            BulkCreateSeatsRequest request)
        {
            if (request.Seats == null ||
                request.Seats.Count == 0)
            {
                throw new ArgumentException(
                    "At least one seat is required.");
            }

            var createdSeats = new List<SeatResponse>();

            foreach (var seatRequest in request.Seats)
            {
                ValidateSeatRequest(seatRequest);

                var exists =
                    await _seatRepository.ExistsAsync(
                        request.ScreenId,
                        seatRequest.RowName,
                        seatRequest.SeatNumber);

                if (exists)
                {
                    throw new InvalidOperationException(
                        $"Seat {seatRequest.RowName}{seatRequest.SeatNumber} already exists.");
                }

                var duplicateInRequest =
                    createdSeats.Any(s =>
                        s.RowName == seatRequest.RowName &&
                        s.SeatNumber == seatRequest.SeatNumber);

                if (duplicateInRequest)
                {
                    throw new InvalidOperationException(
                        $"Duplicate seat {seatRequest.RowName}{seatRequest.SeatNumber} found in the request.");
                }

                var seat = new Seat
                {
                    ScreenId = request.ScreenId,
                    RowName = seatRequest.RowName,
                    SeatNumber = seatRequest.SeatNumber,
                    SeatType = seatRequest.SeatType
                };

                var createdSeat =
                    await _seatRepository.CreateAsync(seat);

                createdSeats.Add(
                    MapToResponse(createdSeat));
            }

            return createdSeats;
        }

        private static void ValidateSeatRequest(
            CreateSeatRequest request)
        {
            if (request.ScreenId <= 0)
                throw new ArgumentException(
                    "Screen ID must be greater than 0.");

            if (request.SeatNumber <= 0)
                throw new ArgumentException(
                    "Seat number must be greater than 0.");

            if (string.IsNullOrWhiteSpace(request.RowName))
                throw new ArgumentException(
                    "Row name is required.");

            if (string.IsNullOrWhiteSpace(request.SeatType))
                throw new ArgumentException(
                    "Seat type is required.");
        }

        private static SeatResponse MapToResponse(
            Seat seat)
        {
            return new SeatResponse
            {
                SeatId = seat.SeatId,
                ScreenId = seat.ScreenId,
                RowName = seat.RowName,
                SeatNumber = seat.SeatNumber,
                SeatType = seat.SeatType
            };
        }
    }
}