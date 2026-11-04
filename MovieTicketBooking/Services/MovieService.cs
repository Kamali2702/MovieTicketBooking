using MovieTicketBooking.DTOs.Movies;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;

        public MovieService(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        public async Task<MovieResponse> CreateAsync(
            CreateMovieRequest request)
        {
            var movie = new Movie
            {
                Title = request.Title,
                Description = request.Description,
                DurationMinutes = request.DurationMinutes,
                ReleaseDate = request.ReleaseDate,
                Language = request.Language,
                Genre = request.Genre
            };

            var createdMovie = await _movieRepository.AddAsync(movie);

            return MapToResponse(createdMovie);
        }

        public async Task<List<MovieResponse>> GetAllAsync(
            int pageNumber,
            int pageSize,
            string? title)
        {
            if (pageNumber < 1)
                throw new ArgumentException(
                    "Page number must be greater than 0.");

            if (pageSize < 1)
                throw new ArgumentException(
                    "Page size must be greater than 0.");

            if (pageSize > 100)
                throw new ArgumentException(
                    "Page size cannot be greater than 100.");

            var movies = await _movieRepository.GetAllAsync(
                pageNumber,
                pageSize,
                title);

            return movies
                .Select(MapToResponse)
                .ToList();
        }

        public async Task<MovieResponse?> GetByIdAsync(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);

            return movie == null ? null : MapToResponse(movie);
        }

        public async Task<MovieResponse?> SearchByTitleAsync(
            string title)
        {
            var movie = await _movieRepository.GetByTitleAsync(title);

            return movie == null ? null : MapToResponse(movie);
        }

        public async Task<bool> UpdateAsync(
            int id,
            UpdateMovieRequest request)
        {
            var movie = await _movieRepository.GetByIdAsync(id);

            if (movie == null)
            {
                return false;
            }

            movie.Title = request.Title;
            movie.Description = request.Description;
            movie.DurationMinutes = request.DurationMinutes;
            movie.ReleaseDate = request.ReleaseDate;
            movie.Language = request.Language;
            movie.Genre = request.Genre;

            await _movieRepository.UpdateAsync(movie);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);

            if (movie == null)
            {
                return false;
            }

            await _movieRepository.DeleteAsync(movie);

            return true;
        }

        private static MovieResponse MapToResponse(Movie movie)
        {
            return new MovieResponse
            {
                MovieId = movie.MovieId,
                Title = movie.Title,
                Description = movie.Description,
                DurationMinutes = movie.DurationMinutes,
                ReleaseDate = movie.ReleaseDate,
                Language = movie.Language,
                Genre = movie.Genre
            };
        }
    }
}