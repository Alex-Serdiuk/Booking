using BookingServer.Models;
using BookingServer.Models.Forms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomNumberController : ControllerBase
    {
        private readonly BookingDbContext _context;
        private readonly ILogger<HotelController> _logger;

        public RoomNumberController(BookingDbContext context, ILogger<HotelController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("createRoomNumber/{roomId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRoomNumber(int roomId, [FromBody] RoomNumberForm roomNumber)
        {
            _logger.LogInformation("Add roomNumber item");
            try
            {
                if (_context.RoomNumbers == null)
                {
                    return Problem("Entity set 'Context.RoomNumbers'  is null.");
                }

                var room = await _context.Rooms.FindAsync(roomId);
                if (room == null)
                {
                    return NotFound();
                }

                var newRoomNumber = new RoomNumber
                {
                    Number = roomNumber.Number,
                    Room = room
                };

                var savedRoomNumber = await _context.RoomNumbers.AddAsync(newRoomNumber);
                await _context.SaveChangesAsync();

                return Ok(savedRoomNumber.Entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut("updateRoomNumber/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRoomNumber(int id, Room roomNumber)
        {
            try
            {
                if (id != roomNumber.Id)
                {
                    return BadRequest(); // якщо ідентифікатори не співпадають, повертаємо 400
                }

                _context.Entry(roomNumber).State = EntityState.Modified; // оновлюємо стан 
                await _context.SaveChangesAsync(); // зберігаємо зміни в базі даних

                return Ok(roomNumber);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("getRoomNumber/{id}")]
        public async Task<IActionResult> GetRoomNumber(int id)
        {
            try
            {
                var roomNumber = await _context.RoomNumbers.FindAsync(id);
                if (roomNumber == null)
                {
                    return NotFound();
                }

                return Ok(roomNumber);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("getRoomNumbers")]
        public async Task<IActionResult> GetRoomNumbers()
        {
            try
            {
                var roomNumbers = await _context.RoomNumbers.ToListAsync();
                return Ok(roomNumbers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRoomNumber(int id)
        {
            try
            {
                var roomNumber = await _context.RoomNumbers.FindAsync(id); // знаходимо roomNumber за ідентифікатором
                if (roomNumber == null)
                {
                    return NotFound(); // якщо roomNumber не знайдена, повертаємо 404
                }
                _context.RoomNumbers.Remove(roomNumber); // видаляємо roomNumber з контексту
                await _context.SaveChangesAsync(); // зберігаємо зміни в базі даних


                return Ok("Room has been deleted.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting room");
                return StatusCode(500, "An error occurred while deleting the room.");
            }
        }

        [HttpPut("availability/{id}")]
        //[Authorize]
        public async Task<IActionResult> UpdateRoomAvailability(int id, [FromBody] DateForm dateList)
        {
            try
            {
                var roomNumber = await _context.RoomNumbers.FindAsync(id); // знаходимо roomNumber за ідентифікатором
                if (roomNumber == null)
                {
                    return NotFound();
                }
                foreach (var item in dateList.Dates)
                {
                    var newDate = new UnavailableDate
                    {
                        Date= item.Date,
                        RoomNumber = roomNumber
                    };
                    await _context.UnavailableDates.AddAsync(newDate);
                }

                await _context.SaveChangesAsync();

                return Ok("Room status has been updated.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating room availability");
                return StatusCode(500, "An error occurred while updating room availability.");
            }
        }
    }
}