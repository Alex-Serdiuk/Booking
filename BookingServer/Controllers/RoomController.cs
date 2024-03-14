using BookingServer.Models;
using BookingServer.Models.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BookingServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly BookingDbContext _context;
        private readonly ILogger<HotelController> _logger;

        public RoomController(BookingDbContext context, ILogger<HotelController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("createRoom/{hotelId}")]
        public async Task<IActionResult> CreateRoom(int hotelId, [FromBody] RoomForm room)
        {
            _logger.LogInformation("Add room item");
            try
            {
                if (_context.Rooms == null)
                {
                    return Problem("Entity set 'Context.Rooms'  is null.");
                }

                var hotel = await _context.Hotels.FindAsync(hotelId);
                if (hotel == null)
                {
                    return NotFound();
                }

                var newRoom = new Room
                {
                    Title = room.Title,
                    Price = room.Price,
                    MaxPeople = room.MaxPeople,
                    Description = room.Description,
                    Hotel = hotel
                };

                var savedRoom = await _context.Rooms.AddAsync(newRoom);
                await _context.SaveChangesAsync();

                foreach (var item in room.RoomNumbers)
                {
                    var newRoomNumber = new RoomNumber
                    {
                        Number = item.Number,
                        Room = savedRoom.Entity
                    };
                    var savedRoomNumber = await _context.RoomNumbers.AddAsync(newRoomNumber);
                }

                await _context.SaveChangesAsync();
                return CreatedAtAction("GetRoom", new { id = savedRoom.Entity.Id }, savedRoom.Entity); // повертаємо 201 і справу
                //return Ok(savedRoom.Entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        //[HttpPost("createRoom/{hotelId}")]
        //public async Task<IActionResult> CreateRoom(int hotelId, [FromBody] Room room)
        //{
        //    _logger.LogInformation("Add room item");
        //    try
        //    {
        //        if (_context.Rooms == null)
        //        {
        //            return Problem("Entity set 'Context.Rooms'  is null.");
        //        }

        //        var hotel = await _context.Hotels.FindAsync(hotelId);
        //        if (hotel == null)
        //        {
        //            return NotFound();
        //        }

        //        var newRoom = new Room
        //        {
        //            Title = room.Title,
        //            Price = room.Price,
        //            MaxPeople = room.MaxPeople,
        //            Description = room.Description,
        //            Hotel = hotel
        //        };

        //        var savedRoom = await _context.Rooms.AddAsync(newRoom);
        //        await _context.SaveChangesAsync();

        //        return Ok(savedRoom.Entity);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"An error occurred: {ex.Message}");
        //    }
        //}

        [HttpPut("updateRoom/{id}")]
        public async Task<IActionResult> UpdateRoom(int id, Room room)
        {
            try
            {
                if (id != room.Id)
                {
                    return BadRequest(); // якщо ідентифікатори не співпадають, повертаємо 400
                }

                _context.Entry(room).State = EntityState.Modified; // оновлюємо стан 
                await _context.SaveChangesAsync(); // зберігаємо зміни в базі даних

                return Ok(room);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("getRoom/{id}")]
        public async Task<IActionResult> GetRoom(int id)
        {
            try
            {
                var room = await _context.Rooms.FindAsync(id);
                if (room == null)
                {
                    return NotFound();
                }

                return Ok(room);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("getRooms")]
        public async Task<IActionResult> GetRooms()
        {
            try
            {
                var rooms = await _context.Rooms.ToListAsync();
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        //[HttpDelete("{hotelid}/{id}")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            try
            {
                var room = await _context.Rooms.FindAsync(id); // знаходимо room за ідентифікатором
                if (room == null)
                {
                    return NotFound(); // якщо room не знайдена, повертаємо 404
                }
                _context.Rooms.Remove(room); // видаляємо room з контексту
                await _context.SaveChangesAsync(); // зберігаємо зміни в базі даних
              

                return Ok("Room has been deleted.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting room");
                return StatusCode(500, "An error occurred while deleting the room.");
            }
        }
    }

}
