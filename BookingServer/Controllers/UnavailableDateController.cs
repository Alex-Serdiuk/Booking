using BookingServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnavailableDateController : ControllerBase
    {
        private readonly BookingDbContext _context;
        private readonly ILogger<HotelController> _logger;

        public UnavailableDateController(BookingDbContext context, ILogger<HotelController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUnavailableDate(int id)
        {
            try
            {
                var date = await _context.UnavailableDates.FindAsync(id); // знаходимо date за ідентифікатором
                if (date == null)
                {
                    return NotFound(); // якщо date не знайдена, повертаємо 404
                }
                _context.UnavailableDates.Remove(date); // видаляємо date з контексту
                await _context.SaveChangesAsync(); // зберігаємо зміни в базі даних


                return Ok("UnavailableDate has been deleted.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting UnavailableDate");
                return StatusCode(500, "An error occurred while deleting the UnavailableDate");
            }
        }
    }
}
