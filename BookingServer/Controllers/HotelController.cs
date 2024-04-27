using BookingServer.Models;
using BookingServer.Models.Forms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BookingServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly BookingDbContext _context;
        private readonly ILogger<HotelController> _logger;

        public HotelController(BookingDbContext context, ILogger<HotelController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Hotel>> CreateHotel([FromBody] HotelForm hotelForm)
        {
            _logger.LogInformation("Add hotel item");
            try
            {
                if (_context.Hotels == null)
                {
                    return Problem("Entity set 'Context.Hotels'  is null.");
                }

                // Конвертуємо об'єкт hotelForm в об'єкт Hotel
                var hotel = new Hotel
                {
                    Name = hotelForm.Name,
                    Type = hotelForm.Type,
                    City = hotelForm.City,
                    Address = hotelForm.Address,
                    Distance = hotelForm.Distance,
                    Title = hotelForm.Title,
                    Description = hotelForm.Description,
                    //CheapestPrice = hotelForm.CheapestPrice,
                    Featured = hotelForm.Featured
                };

                _context.Hotels.Add(hotel);
                await _context.SaveChangesAsync(); // зберігаємо зміни в базі даних

                // Перевіряємо, чи передано фотографії готелю
                if (hotelForm.Photos != null)
                {
                    foreach (var photoUrl in hotelForm.Photos)
                    {
                        // Перевіряємо URL фотографії
                        if (Uri.TryCreate(photoUrl, UriKind.Absolute, out Uri validatedUri))
                        {
                            var hotelImage = new HotelImage
                            { 
                                Url = photoUrl,
                                Hotel = hotel
                            };
                            hotel.HotelImages.Add(hotelImage);
                        }
                        else
                        {
                            // Якщо URL недійсний, ігноруємо його або видаємо помилку, якщо потрібно
                            // Можна вибрати одну з наступних стратегій:
                            // - Пропустити недійсні URL і продовжити створення готелю
                            // - Повернути помилку клієнту, якщо вважаєте, що це критична помилка
                            // В даному прикладі просто ігноруємо недійсні URL
                            _logger.LogWarning($"Invalid URL: {photoUrl}");
                        }
                    }
                }
                await _context.SaveChangesAsync(); // зберігаємо зміни в базі даних

                // Додамо кімнати до готелю, якщо вони передані
                if (hotelForm.Rooms != null)
                {
                    foreach (var roomForm in hotelForm.Rooms)
                    {
                        var room = await _context.Rooms.FindAsync(roomForm);
                        var newRoom = new Room
                        {
                            Title = room.Title,
                            Price = room.Price,
                            MaxPeople = room.MaxPeople,
                            Description = room.Description,
                            Hotel = hotel
                        };
                        _context.Rooms.Add(newRoom);
                    }
                }
                await _context.SaveChangesAsync(); // зберігаємо зміни в базі даних

                return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);// повертаємо 201 і готель
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating hotel: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateHotel(int id, [FromBody] HotelForm hotelForm)
        {
            try
            {
                var existingHotel = await _context.Hotels.FindAsync(id); // Знаходимо готель за ідентифікатором
                if (existingHotel == null)
                {
                    return NotFound(); // Якщо готель не знайдено, повертаємо 404
                }
                // Конвертуємо об'єкт hotelForm в об'єкт Hotel

                existingHotel.Name = hotelForm.Name;
                existingHotel.Type = hotelForm.Type;
                existingHotel.City = hotelForm.City;
                existingHotel.Address = hotelForm.Address;
                existingHotel.Distance = hotelForm.Distance;
                existingHotel.Title = hotelForm.Title;
                existingHotel.Description = hotelForm.Description;
                //existingHotel.CheapestPrice = hotelForm.CheapestPrice;
                existingHotel.Featured = hotelForm.Featured;

                // Перевіряємо, чи передано фотографії готелю
                if (hotelForm.Photos != null)
                {
                    foreach (var photoUrl in hotelForm.Photos)
                    {
                        // Перевіряємо URL фотографії
                        if (Uri.TryCreate(photoUrl, UriKind.Absolute, out Uri validatedUri))
                        {
                            var hotelImage = new HotelImage
                            {
                                Url = photoUrl,
                                Hotel = existingHotel
                            };
                            existingHotel.HotelImages.Add(hotelImage);
                        }
                        else
                        {
                            // Якщо URL недійсний, ігноруємо його або видаємо помилку, якщо потрібно
                            // Можна вибрати одну з наступних стратегій:
                            // - Пропустити недійсні URL і продовжити створення готелю
                            // - Повернути помилку клієнту, якщо вважаєте, що це критична помилка
                            // В даному прикладі просто ігноруємо недійсні URL
                            _logger.LogWarning($"Invalid URL: {photoUrl}");
                        }
                    }
                }

                // Додамо кімнати до готелю, якщо вони передані
                if (hotelForm.Rooms != null)
                {
                    foreach (var roomForm in hotelForm.Rooms)
                    {
                        var room = await _context.Rooms.FindAsync(roomForm);
                        var newRoom = new Room
                        {
                            Title = room.Title,
                            Price = room.Price,
                            MaxPeople = room.MaxPeople,
                            Description = room.Description,
                            Hotel = existingHotel
                        };
                        _context.Rooms.Add(newRoom);
                    }
                }

                //_context.Entry(hotel).State = EntityState.Modified; // оновлюємо стан готелю
                await _context.SaveChangesAsync(); // зберігаємо зміни в базі даних

                return Ok(existingHotel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating hotel: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            try
            {
                var hotel = await _context.Hotels.FindAsync(id); // знаходимо готель за ідентифікатором
                if (hotel == null)
                {
                    return NotFound(); // якщо готель не знайдена, повертаємо 404
                }
                _context.Hotels.Remove(hotel); // видаляємо готель з контексту
                await _context.SaveChangesAsync(); // зберігаємо зміни в базі даних
                return Ok("Hotel has been deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting hotel: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Hotel>> GetHotel(int id)
        {
            // знаходимо готель за ідентифікатором
            //var hotel = await _context.Hotels.FindAsync(id);
            var hotel = await _context.Hotels
                .Include(h => h.HotelImages) // включаємо список зображень для готелю
                .Include(h => h.Rooms)       // Include rooms to compute CheapestPrice
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hotel == null)
            {
                return NotFound("Hotel not found."); // якщо готель не знайдено, повертаємо 404
            }

            return Ok(hotel); // якщо готель знайдено, повертаємо його
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hotel>>> GetHotels([FromQuery] HotelFilter filter)
        {
            IQueryable<Hotel> hotelsQuery = _context.Hotels
                .Include(h => h.HotelImages)
                .Include(h => h.Rooms);    // Preload rooms to compute CheapestPrice;

            // Retrieve hotels first to filter them in memory
            var hotels = await hotelsQuery.ToListAsync();

            // Now apply filters on the in-memory collection
            if (filter.Max != null && filter.Min != null)
            {
                hotels = hotels.Where(h => h.Rooms.Any() && h.Rooms.Min(r => r.Price) >= filter.Min && h.Rooms.Min(r => r.Price) <= filter.Max).ToList();
            }

            if (filter.Featured != null)
            {
                hotels = hotels.Where(h => h.Featured == filter.Featured).ToList();
            }

            if (filter.City != null)
            {
                hotels = hotels.Where(h => h.City == filter.City).ToList();
            }

            if (filter.Name != null)
            {
                hotels = hotels.Where(h => h.Name == filter.Name).ToList();
            }

            if (filter.Limit != null)
            {
                hotels = hotels.Take(filter.Limit.Value).ToList();
            }

            if (!hotels.Any())
            {
                return NotFound("No hotels found.");
            }

            return Ok(hotels);
        }

        [HttpGet("countByCity")]
        public async Task<IActionResult> CountByCity([FromQuery] string cities)
        {
            try
            {
                var cityList = cities.Split(",");
                var counts = new Dictionary<string, int>();

                foreach (var city in cityList)
                {
                    var count = await _context.Hotels.CountAsync(h => h.City == city);
                    counts.Add(city, count);
                }

                return Ok(counts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("countByType")]
        public async Task<IActionResult> CountByType()
        {
            try
            {
                var counts = new List<HotelTypeCount>();

                var hotelCount = await _context.Hotels.CountAsync(h => h.Type == "hotel");
                counts.Add(new HotelTypeCount { Type = "hotel", Count = hotelCount });

                var apartmentCount = await _context.Hotels.CountAsync(h => h.Type == "apartment");
                counts.Add(new HotelTypeCount { Type = "apartments", Count = apartmentCount });

                var resortCount = await _context.Hotels.CountAsync(h => h.Type == "resort");
                counts.Add(new HotelTypeCount { Type = "resorts", Count = resortCount });

                var villaCount = await _context.Hotels.CountAsync(h => h.Type == "villa");
                counts.Add(new HotelTypeCount { Type = "villas", Count = villaCount });

                var cabinCount = await _context.Hotels.CountAsync(h => h.Type == "cabin");
                counts.Add(new HotelTypeCount { Type = "cabins", Count = cabinCount });

                return Ok(counts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("GetRoomsByHotelId/{id}")]
        public async Task<IActionResult> GetRoomsByHotelId(int id)
        {
            try
            {
                var rooms = await _context.Rooms
                    .Include(r => r.RoomNumbers)
                        .ThenInclude(rn => rn.UnavailableDates)
                    .Where(r => r.Hotel.Id == id)
                    .ToListAsync();


                if (rooms == null)
                {
                    return NotFound();
                }

                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("GetHotelImagesByHotelId/{id}")]
        public async Task<IActionResult> GetHotelImagesByHotelId(int id)
        {
            try
            {
                var hotelImages = await _context.HotelImages
                    .Where(hImg => hImg.Hotel.Id == id)
                    .ToListAsync();


                if (hotelImages == null)
                {
                    return NotFound();
                }

                return Ok(hotelImages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }

}