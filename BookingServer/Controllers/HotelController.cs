using BookingServer.Models;
using BookingServer.Models.Forms;
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
        public async Task<ActionResult<Hotel>> CreateHotel([FromBody] Hotel hotel)
        {
            _logger.LogInformation("Add hotel item");
            try
            {
                if (_context.Hotels == null)
                {
                    return Problem("Entity set 'Context.Hotels'  is null.");
                }
                _context.Hotels.Add(hotel);
                await _context.SaveChangesAsync(); // зберігаємо зміни в базі даних

                return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);// повертаємо 201 і готель
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating hotel: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHotel(int id, Hotel hotel)
        {
            try
            {
                if (id != hotel.Id)
                {
                    return BadRequest(); // якщо ідентифікатори не співпадають, повертаємо 400
                }

                _context.Entry(hotel).State = EntityState.Modified; // оновлюємо стан готелю
                await _context.SaveChangesAsync(); // зберігаємо зміни в базі даних

                return Ok(hotel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating hotel: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
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
            var hotel = await _context.Hotels.FindAsync(id);

            if (hotel == null)
            {
                return NotFound(); // якщо готель не знайдено, повертаємо 404
            }

            return Ok(hotel); // якщо готель знайдено, повертаємо його
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hotel>>> GetHotels([FromQuery] HotelFilter filter)
        {
            var hotels = new List<Hotel>();
            if (filter.Max != null && filter.Min != null)
            {
                hotels = await _context.Hotels.Where(h => h.CheapestPrice > (filter.Min == 1 ? 1 : filter.Min) && h.CheapestPrice < (filter.Max == 999 ? 999 : filter.Max)).ToListAsync();
            }
            else
            {
                hotels = await _context.Hotels.ToListAsync();
            }


            if (hotels == null)
            {
                return NotFound();
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

        [HttpGet("/room/{id}")]
        public async Task<IActionResult> GetHotelRooms(int id)
        {
            try
            {
                var rooms = await _context.Rooms
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
    }

}