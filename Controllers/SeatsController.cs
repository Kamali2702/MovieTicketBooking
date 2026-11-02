using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [ApiController]
    [Route("api/seats")]
    public class SeatsController : ControllerBase
    {
        private readonly ISeatService _seatService;

        public SeatsController(ISeatService seatService)
        {
            _seatService = seatService;
        }

        // POST: api/seats
        [HttpPost]
        public async Task<IActionResult> Create(CreateSeatRequest request)
        {
            var seat = await _seatService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = seat.SeatId },
                seat);
        }

        // POST: api/seats/bulk
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkCreate(
            BulkCreateSeatsRequest request)
        {
            var seats = await _seatService.BulkCreateAsync(request);

            return StatusCode(StatusCodes.Status201Created, seats);
        }

        // GET: api/seats/screen/{screenId}
        [HttpGet("screen/{screenId}")]
        public async Task<IActionResult> GetByScreenId(int screenId)
        {
            var seats = await _seatService.GetByScreenIdAsync(screenId);

            return Ok(seats);
        }

        // GET: api/seats/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var seat = await _seatService.GetByIdAsync(id);

            if (seat == null)
                return NotFound();

            return Ok(seat);
        }

        // PUT: api/seats/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateSeatRequest request)
        {
            await _seatService.UpdateAsync(id, request);

            return NoContent();
        }

        // DELETE: api/seats/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _seatService.DeleteAsync(id);

            return NoContent();
        }
    }
}