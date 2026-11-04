using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Screens;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [ApiController]
    [Route("api/screens")]
    public class ScreensController : ControllerBase
    {
        private readonly IScreenService _screenService;

        public ScreensController(IScreenService screenService)
        {
            _screenService = screenService;
        }

        // POST: api/screens
        [HttpPost]
        public async Task<IActionResult> Create(
            CreateScreenRequest request)
        {
            var screen =
                await _screenService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = screen.ScreenId },
                screen);
        }

        // GET: api/screens
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var screens =
                await _screenService.GetAllAsync();

            return Ok(screens);
        }

        // GET: api/screens/theatre/{theatreId}
        [HttpGet("theatre/{theatreId}")]
        public async Task<IActionResult> GetByTheatre(int theatreId)
        {
            var screens =
                await _screenService.GetByTheatreIdAsync(theatreId);

            return Ok(screens);
        }

        // GET: api/screens/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var screen =
                await _screenService.GetByIdAsync(id);

            if (screen == null)
                return NotFound();

            return Ok(screen);
        }

        // PUT: api/screens/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateScreenRequest request)
        {
            await _screenService.UpdateAsync(id, request);

            return NoContent();
        }

        // DELETE: api/screens/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _screenService.DeleteAsync(id);

            return NoContent();
        }
    }
}