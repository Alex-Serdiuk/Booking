using BookingServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomImageController : ControllerBase
    {
        private readonly BookingDbContext _context;
        private readonly ILogger<HotelController> _logger;

        public RoomImageController(BookingDbContext context, ILogger<HotelController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRoomImage(int id)
        {
            try
            {
                var roomImage = await _context.RoomImages.FindAsync(id); // знаходимо image за ідентифікатором
                if (roomImage == null)
                {
                    return NotFound(); // якщо image не знайдений, повертаємо 404
                }
                _context.RoomImages.Remove(roomImage); // видаляємо image з контексту
                await _context.SaveChangesAsync(); // зберігаємо зміни в базі даних
                return Ok("RoomImage has been deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting roomImage: {ex.Message}");
            }
        }
    }
}
