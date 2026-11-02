using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<BookingResponse> CreateBookingAsync(
            CreateBookingRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CustomerName))
                throw new InvalidOperationException(
                    "Customer name is required.");

            if (string.IsNullOrWhiteSpace(request.CustomerEmail))
                throw new InvalidOperationException(
                    "Customer email is required.");

            if (string.IsNullOrWhiteSpace(request.CustomerPhone))
                throw new InvalidOperationException(
                    "Customer phone is required.");

            if (request.ShowId <= 0)
                throw new InvalidOperationException(
                    "Valid ShowId is required.");

            if (request.SeatIds == null || request.SeatIds.Count == 0)
                throw new InvalidOperationException(
                    "At least one seat must be selected.");

            if (request.SeatIds.Distinct().Count() != request.SeatIds.Count)
                throw new InvalidOperationException(
                    "Duplicate seats are not allowed.");

            var show = await _bookingRepository
                .GetShowWithShowSeatsAsync(request.ShowId);

            if (show == null)
                throw new KeyNotFoundException("Show not found.");

            var requestedSeatIds = request.SeatIds.ToHashSet();

            var selectedShowSeats = show.ShowSeats
                .Where(ss => requestedSeatIds.Contains(ss.SeatId))
                .ToList();

            if (selectedShowSeats.Count != request.SeatIds.Count)
                throw new InvalidOperationException(
                    "One or more selected seats are not available for this show.");

            if (selectedShowSeats.Any(ss =>
                !string.Equals(
                    ss.Status,
                    "Available",
                    StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    "One or more selected seats are already booked.");
            }

            var seats = await _bookingRepository
                .GetSeatsByIdsAsync(request.SeatIds);

            if (seats.Count != request.SeatIds.Count)
                throw new KeyNotFoundException(
                    "One or more seats were not found.");

            decimal totalAmount =
                show.TicketPrice * request.SeatIds.Count;

            var booking = new Booking
            {
                BookingReference = GenerateBookingReference(),
                ShowId = show.ShowId,
                CustomerName = request.CustomerName.Trim(),
                CustomerEmail = request.CustomerEmail.Trim(),
                CustomerPhone = request.CustomerPhone.Trim(),
                BookingDate = DateTime.Now,
                TotalAmount = totalAmount,
                Status = "Confirmed"
            };

            foreach (var seatId in request.SeatIds)
            {
                booking.BookingSeats.Add(new BookingSeat
                {
                    SeatId = seatId,
                    Price = show.TicketPrice
                });
            }

            await _bookingRepository.BeginTransactionAsync();

            try
            {
                await _bookingRepository.CreateAsync(booking);

                foreach (var showSeat in selectedShowSeats)
                {
                    showSeat.Status = "Booked";
                }

                await _bookingRepository.SaveChangesAsync();
                await _bookingRepository.CommitTransactionAsync();

                return MapToResponse(booking, seats);
            }
            catch
            {
                await _bookingRepository.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<BookingResponse> GetBookingByIdAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);

            if (booking == null)
                throw new KeyNotFoundException("Booking not found.");

            return MapToResponse(
                booking,
                booking.BookingSeats
                    .Select(bs => bs.Seat)
                    .ToList());
        }

        public async Task<BookingResponse> GetBookingByReferenceAsync(
            string bookingReference)
        {
            if (string.IsNullOrWhiteSpace(bookingReference))
                throw new InvalidOperationException(
                    "Booking reference is required.");

            var booking = await _bookingRepository
                .GetByReferenceAsync(bookingReference);

            if (booking == null)
                throw new KeyNotFoundException("Booking not found.");

            return MapToResponse(
                booking,
                booking.BookingSeats
                    .Select(bs => bs.Seat)
                    .ToList());
        }

        public async Task<BookingResponse> CancelBookingAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);

            if (booking == null)
                throw new KeyNotFoundException("Booking not found.");

            if (!string.Equals(
                booking.Status,
                "Confirmed",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Only confirmed bookings can be cancelled.");
            }

            var show = await _bookingRepository
                .GetShowWithShowSeatsAsync(booking.ShowId);

            if (show == null)
                throw new KeyNotFoundException("Show not found.");

            var bookedSeatIds = booking.BookingSeats
                .Select(bs => bs.SeatId)
                .ToHashSet();

            var showSeats = show.ShowSeats
                .Where(ss => bookedSeatIds.Contains(ss.SeatId))
                .ToList();

            await _bookingRepository.BeginTransactionAsync();

            try
            {
                booking.Status = "Cancelled";

                foreach (var showSeat in showSeats)
                {
                    showSeat.Status = "Available";
                }

                await _bookingRepository.SaveChangesAsync();
                await _bookingRepository.CommitTransactionAsync();

                return MapToResponse(
                    booking,
                    booking.BookingSeats
                        .Select(bs => bs.Seat)
                        .ToList());
            }
            catch
            {
                await _bookingRepository.RollbackTransactionAsync();
                throw;
            }
        }

        private static string GenerateBookingReference()
        {
            return $"MOV{Guid.NewGuid():N}"[..15].ToUpper();
        }

        private static BookingResponse MapToResponse(
            Booking booking,
            List<Seat> seats)
        {
            return new BookingResponse
            {
                BookingId = booking.BookingId,
                BookingReference = booking.BookingReference,
                ShowId = booking.ShowId,
                TotalAmount = booking.TotalAmount,
                Status = booking.Status,
                Seats = seats
                    .OrderBy(s => s.RowName)
                    .ThenBy(s => s.SeatNumber)
                    .Select(s => $"{s.RowName}{s.SeatNumber}")
                    .ToList()
            };
        }
    }
}