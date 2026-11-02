using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [ApiController]
    [Route("api/shows")]
    public class ShowsController : ControllerBase
    {
        private readonly IShowService _showService;

        public ShowsController(IShowService showService)
        {
            _showService = showService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateShowRequest request)
        {
            var response = await _showService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = response.ShowId },
                response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] DateTime? date,
            [FromQuery] int? movieId,
            [FromQuery] int? theatreId)
        {
            if (date.HasValue)
            {
                var shows = await _showService.GetByDateAsync(date.Value);
                return Ok(shows);
            }

            if (movieId.HasValue)
            {
                var shows = await _showService.GetByMovieIdAsync(movieId.Value);
                return Ok(shows);
            }

            if (theatreId.HasValue)
            {
                var shows = await _showService.GetByTheatreIdAsync(theatreId.Value);
                return Ok(shows);
            }

            var allShows = await _showService.GetAllAsync();

            return Ok(allShows);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var show = await _showService.GetByIdAsync(id);

            if (show == null)
            {
                return NotFound(new
                {
                    message = "Show not found."
                });
            }

            return Ok(show);
        }

        [HttpGet("movie/{movieId}")]
        public async Task<IActionResult> GetByMovieId(int movieId)
        {
            var shows = await _showService.GetByMovieIdAsync(movieId);

            return Ok(shows);
        }

        [HttpGet("theatre/{theatreId}")]
        public async Task<IActionResult> GetByTheatreId(int theatreId)
        {
            var shows = await _showService.GetByTheatreIdAsync(theatreId);

            return Ok(shows);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] DateTime date)
        {
            var shows = await _showService.GetByDateAsync(date);

            return Ok(shows);
        }

        [HttpGet("{id}/seats")]
        public async Task<IActionResult> GetShowSeats(int id)
        {
            var seats = await _showService.GetShowSeatsAsync(id);

            return Ok(seats);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateShowRequest request)
        {
            await _showService.UpdateAsync(id, request);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _showService.DeleteAsync(id);

            return NoContent();
        }
    }
}