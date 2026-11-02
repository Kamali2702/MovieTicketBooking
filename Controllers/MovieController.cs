using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Movies;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpPost]
        public async Task<ActionResult<MovieResponse>> Create(
            CreateMovieRequest request)
        {
            var movie = await _movieService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = movie.MovieId },
                movie);
        }

        [HttpGet]
        public async Task<ActionResult<List<MovieResponse>>> GetAll()
        {
            var movies = await _movieService.GetAllAsync();

            return Ok(movies);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MovieResponse>> GetById(int id)
        {
            var movie = await _movieService.GetByIdAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            return Ok(movie);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateMovieRequest request)
        {
            var updated = await _movieService.UpdateAsync(id, request);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _movieService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<MovieResponse>> Search(
            [FromQuery] string title)
        {
            var movie = await _movieService.SearchByTitleAsync(title);

            if (movie == null)
            {
                return NotFound();
            }

            return Ok(movie);
        }
    }
}