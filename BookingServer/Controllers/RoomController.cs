using BookingServer.Models;
using BookingServer.Models.Forms;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin")]
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

                if (room.RoomNumbers.Any())
                {
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
                }

                // Перевіряємо, чи передано фотографії номеру
                if (room.Photos != null)
                {
                    foreach (var photoUrl in room.Photos)
                    {
                        // Перевіряємо URL фотографії
                        if (Uri.TryCreate(photoUrl, UriKind.Absolute, out Uri validatedUri))
                        {
                            var roomImage = new RoomImage
                            {
                                Url = photoUrl,
                                Room = newRoom
                            };
                            newRoom.RoomImages.Add(roomImage);
                        }
                        else
                        {
                            // Якщо URL недійсний, ігноруємо його або видаємо помилку, якщо потрібно
                            // Можна вибрати одну з наступних стратегій:
                            // - Пропустити недійсні URL і продовжити створення номеру
                            // - Повернути помилку клієнту, якщо вважаєте, що це критична помилка
                            // В даному прикладі просто ігноруємо недійсні URL
                            _logger.LogWarning($"Invalid URL: {photoUrl}");
                        }
                    }
                }
                await _context.SaveChangesAsync(); // зберігаємо зміни в базі даних
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

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRoom(int id, [FromBody] RoomForm room)
        {
            _logger.LogInformation("Add room item");
            try
            {
                var existingRoom = await _context.Rooms.FindAsync(id); // Знаходимо room за ідентифікатором
                if (existingRoom == null)
                {
                    return NotFound(); // Якщо room не знайдений, повертаємо 404
                }
                // Оновлюємо дані room з відповідними даними з запиту
                existingRoom.Title = room.Title;
                existingRoom.Description = room.Description;
                existingRoom.Price = room.Price;
                existingRoom.MaxPeople = room.MaxPeople;

                _context.Rooms.Update(existingRoom); // оновлюємо стан 
                await _context.SaveChangesAsync(); // зберігаємо зміни в базі даних

                if (room.RoomNumbers.Any())
                {
                    foreach (var item in room.RoomNumbers)
                    {
                        var newRoomNumber = new RoomNumber
                        {
                            Number = item.Number,
                            Room = existingRoom
                        };
                        var savedRoomNumber = await _context.RoomNumbers.AddAsync(newRoomNumber);
                    }

                    await _context.SaveChangesAsync();
                }

                // Перевіряємо, чи передано фотографії номеру
                if (room.Photos != null)
                {
                    foreach (var photoUrl in room.Photos)
                    {
                        // Перевіряємо URL фотографії
                        if (Uri.TryCreate(photoUrl, UriKind.Absolute, out Uri validatedUri))
                        {
                            var roomImage = new RoomImage
                            {
                                Url = photoUrl,
                                Room = existingRoom
                            };
                            existingRoom.RoomImages.Add(roomImage);
                        }
                        else
                        {
                            // Якщо URL недійсний, ігноруємо його або видаємо помилку, якщо потрібно
                            // Можна вибрати одну з наступних стратегій:
                            // - Пропустити недійсні URL і продовжити створення номеру
                            // - Повернути помилку клієнту, якщо вважаєте, що це критична помилка
                            // В даному прикладі просто ігноруємо недійсні URL
                            _logger.LogWarning($"Invalid URL: {photoUrl}");
                        }
                    }
                }
                await _context.SaveChangesAsync(); // зберігаємо зміни в базі даних


                return Ok(existingRoom);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoom(int id)
        {
            try
            {
                var room = await _context.Rooms
                    .Include(r => r.RoomImages) // включаємо список зображень для room
                .Include(r => r.RoomNumbers)       // Include roomNumbers
                .FirstOrDefaultAsync(r => r.Id == id);
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

        [HttpGet]
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
        [Authorize(Roles = "Admin")]
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

        [HttpGet("GetRoomNumbersByRoomId/{id}")]
        public async Task<IActionResult> GetRoomNumbersByRoomId(int id)
        {
            try
            {
                var roomNumbers = await _context.RoomNumbers
                    .Where(rNum => rNum.Room.Id == id)
                    .ToListAsync();


                if (roomNumbers == null)
                {
                    return NotFound();
                }

                return Ok(roomNumbers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("GetRoomImagesByRoomId/{id}")]
        public async Task<IActionResult> GetRoomImagesByRoomId(int id)
        {
            try
            {
                var roomImages = await _context.RoomImages
                    .Where(rImg => rImg.Room.Id == id)
                    .ToListAsync();


                if (roomImages == null)
                {
                    return NotFound();
                }

                return Ok(roomImages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }

}
