using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Theatres;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TheatresController : ControllerBase
    {
        private readonly ITheatreService _theatreService;

        public TheatresController(ITheatreService theatreService)
        {
            _theatreService = theatreService;
        }

        [HttpPost]
        public async Task<ActionResult<TheatreResponse>> Create(
            CreateTheatreRequest request)
        {
            var theatre = await _theatreService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = theatre.TheatreId },
                theatre);
        }

        [HttpGet]
        public async Task<ActionResult<List<TheatreResponse>>> GetAll()
        {
            var theatres = await _theatreService.GetAllAsync();

            return Ok(theatres);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TheatreResponse>> GetById(int id)
        {
            var theatre = await _theatreService.GetByIdAsync(id);

            if (theatre == null)
                return NotFound();

            return Ok(theatre);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateTheatreRequest request)
        {
            var updated = await _theatreService.UpdateAsync(id, request);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _theatreService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}