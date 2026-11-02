using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Screens;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScreensController : ControllerBase
    {
        private readonly IScreenService _screenService;

        public ScreensController(IScreenService screenService)
        {
            _screenService = screenService;
        }

        [HttpPost]
        public async Task<ActionResult<ScreenResponse>> Create(
            CreateScreenRequest request)
        {
            var screen = await _screenService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = screen.ScreenId },
                screen);
        }

        [HttpGet]
        public async Task<ActionResult<List<ScreenResponse>>> GetAll()
        {
            var screens = await _screenService.GetAllAsync();

            return Ok(screens);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ScreenResponse>> GetById(int id)
        {
            var screen = await _screenService.GetByIdAsync(id);

            if (screen == null)
                return NotFound();

            return Ok(screen);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateScreenRequest request)
        {
            var updated = await _screenService.UpdateAsync(id, request);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _screenService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}