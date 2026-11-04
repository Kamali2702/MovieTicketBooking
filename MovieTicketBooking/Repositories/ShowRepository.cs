using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class ShowRepository : IShowRepository
    {
        private readonly ApplicationDbContext _context;

        public ShowRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Show> CreateAsync(Show show)
        {
            _context.Shows.Add(show);
            await _context.SaveChangesAsync();

            return show;
        }

        public async Task<List<Show>> GetAllAsync(
            int pageNumber,
            int pageSize,
            DateTime? date,
            int? movieId,
            int? theatreId)
        {
            var query = _context.Shows
                .AsNoTracking()
                .AsQueryable();

            if (date.HasValue)
            {
                var startDate = date.Value.Date;
                var endDate = startDate.AddDays(1);

                query = query.Where(s =>
                    s.StartTime >= startDate &&
                    s.StartTime < endDate);
            }

            if (movieId.HasValue)
            {
                query = query.Where(s =>
                    s.MovieId == movieId.Value);
            }

            if (theatreId.HasValue)
            {
                query = query.Where(s =>
                    s.Screen.TheatreId == theatreId.Value);
            }

            return await query
                .OrderBy(s => s.StartTime)
                .ThenBy(s => s.ShowId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Show?> GetByIdAsync(int id)
        {
            return await _context.Shows
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ShowId == id);
        }

        public async Task<List<Show>> GetByMovieIdAsync(int movieId)
        {
            return await _context.Shows
                .AsNoTracking()
                .Where(s => s.MovieId == movieId)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<List<Show>> GetByTheatreIdAsync(int theatreId)
        {
            return await _context.Shows
                .AsNoTracking()
                .Where(s => s.Screen.TheatreId == theatreId)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<List<Show>> GetByDateAsync(DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            return await _context.Shows
                .AsNoTracking()
                .Where(s =>
                    s.StartTime >= startDate &&
                    s.StartTime < endDate)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<bool> HasOverlapAsync(
            int screenId,
            DateTime startTime,
            DateTime endTime,
            int? excludeShowId = null)
        {
            return await _context.Shows.AnyAsync(s =>
                s.ScreenId == screenId &&
                (!excludeShowId.HasValue ||
                 s.ShowId != excludeShowId.Value) &&
                s.StartTime < endTime &&
                startTime < s.EndTime);
        }

        public async Task<List<Seat>> GetSeatsByScreenIdAsync(int screenId)
        {
            return await _context.Seats
                .AsNoTracking()
                .Where(s => s.ScreenId == screenId)
                .OrderBy(s => s.RowName)
                .ThenBy(s => s.SeatNumber)
                .ToListAsync();
        }

        public async Task<List<ShowSeat>> GetShowSeatsByShowIdAsync(int showId)
        {
            return await _context.ShowSeats
                .Where(ss => ss.ShowId == showId)
                .ToListAsync();
        }

        public async Task AddShowSeatsAsync(List<ShowSeat> showSeats)
        {
            _context.ShowSeats.AddRange(showSeats);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Show show)
        {
            _context.Shows.Update(show);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteShowSeatsAsync(List<ShowSeat> showSeats)
        {
            _context.ShowSeats.RemoveRange(showSeats);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Show show)
        {
            _context.Shows.Remove(show);
            await _context.SaveChangesAsync();
        }
    }
}