using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<Show?> GetShowWithShowSeatsAsync(int showId);

        Task<List<Seat>> GetSeatsByIdsAsync(List<int> seatIds);

        Task<Booking> CreateAsync(Booking booking);

        Task<Booking?> GetByIdAsync(int id);

        Task<Booking?> GetByReferenceAsync(string bookingReference);

        Task UpdateAsync(Booking booking);

        Task SaveChangesAsync();

        Task BeginTransactionAsync();

        Task CommitTransactionAsync();

        Task RollbackTransactionAsync();
    }
}