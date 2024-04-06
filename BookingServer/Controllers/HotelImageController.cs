using BookingServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelImageController : ControllerBase
    {
        private readonly BookingDbContext _context;
        private readonly ILogger<HotelController> _logger;

        public HotelImageController(BookingDbContext context, ILogger<HotelController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteHotelImage(int id)
        {
            try
            {
                var hotelImage = await _context.HotelImages.FindAsync(id); // знаходимо image за ідентифікатором
                if (hotelImage == null)
                {
                    return NotFound(); // якщо image не знайдений, повертаємо 404
                }
                _context.HotelImages.Remove(hotelImage); // видаляємо image з контексту
                await _context.SaveChangesAsync(); // зберігаємо зміни в базі даних
                return Ok("HotelImage has been deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting hotel: {ex.Message}");
            }
        }
    }
}
