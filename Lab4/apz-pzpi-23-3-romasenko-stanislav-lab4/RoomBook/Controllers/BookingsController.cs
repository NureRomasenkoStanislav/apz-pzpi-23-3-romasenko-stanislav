using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomBook.API.DTOs;
using RoomBook.Core.Entities;
using RoomBook.Core.Interfaces;

namespace RoomBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingsController(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        
        [HttpGet("available")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Room>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAvailableRooms([FromQuery] AvailabilityQueryDto query)
        {
            if (query.StartTime >= query.EndTime)
            {
                return BadRequest("Час початку бронювання має бути раніше часу закінчення.");
            }

            var availableRooms = await _bookingRepository.GetAvailableRoomsAsync(
                query.StartTime,
                query.EndTime,
                query.MinCapacity
            );

            return Ok(availableRooms);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequestDto request)
        {
            if (request.StartTime >= request.EndTime || request.RoomId <= 0 || request.UserId <= 0)
            {
                return BadRequest("Некоректні дані бронювання (StartTime >= EndTime або недійсний ID).");
            }

            var availableRooms = await _bookingRepository.GetAvailableRoomsAsync(
                request.StartTime,
                request.EndTime,
                1 
            );

            if (!availableRooms.Any(r => r.RoomId == request.RoomId))
            {
                return BadRequest("Вибране приміщення недоступне на цей час.");
            }

            var newBooking = new Booking
            {
                UserId = request.UserId,
                RoomId = request.RoomId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Purpose = request.Purpose,
                CreationTime = DateTime.Now,
                IsConfirmed = true
            };

            var createdBooking = await _bookingRepository.CreateBookingAsync(newBooking);

            return CreatedAtAction(nameof(GetBookingById), new { bookingId = createdBooking.BookingId }, createdBooking);
        }

        [HttpGet("{bookingId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBookingById(int bookingId)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(bookingId);

            if (booking == null)
            {
                return NotFound();
            }

            return Ok(booking);
        }

        [HttpPut("{bookingId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateBooking(int bookingId, [FromBody] BookingRequestDto request)
        {
            if (bookingId <= 0 || request.StartTime >= request.EndTime || request.RoomId <= 0)
            {
                return BadRequest("Некоректні дані для оновлення бронювання.");
            }

            var existingBooking = await _bookingRepository.GetBookingByIdAsync(bookingId);
            if (existingBooking == null)
            {
                return NotFound($"Бронювання з ID {bookingId} не знайдено.");
            }

            existingBooking.RoomId = request.RoomId;
            existingBooking.StartTime = request.StartTime;
            existingBooking.EndTime = request.EndTime;
            existingBooking.Purpose = request.Purpose;
            existingBooking.UserId = request.UserId;

            var updated = await _bookingRepository.UpdateBookingAsync(existingBooking);

            if (!updated)
            {
                return BadRequest("Не вдалося оновити бронювання.");
            }

            return NoContent();
        }

        [HttpDelete("{bookingId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBooking(int bookingId)
        {
            var deleted = await _bookingRepository.DeleteBookingAsync(bookingId);

            if (!deleted)
            {
                return NotFound($"Бронювання з ID {bookingId} не знайдено.");
            }

            return NoContent();
        }
    }
}