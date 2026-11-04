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
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? date = null,
            [FromQuery] int? movieId = null,
            [FromQuery] int? theatreId = null)
        {
            var shows = await _showService.GetAllAsync(
                pageNumber,
                pageSize,
                date,
                movieId,
                theatreId);

            return Ok(shows);
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