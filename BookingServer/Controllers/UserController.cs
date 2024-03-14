using BookingServer.Models;
using BookingServer.Models.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly BookingDbContext _context;
        private readonly ILogger<HotelController> _logger;

        public UserController(BookingDbContext context, ILogger<HotelController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateUser(int id, [FromBody] RegisterForm user)
        //{
        //    try
        //    {
        //        if (id != user.Id)
        //        {
        //            return BadRequest(); // якщо ідентифікатори не співпадають, повертаємо 400
        //        }

        //        _context.Entry(user).State = EntityState.Modified; // оновлюємо стан 
        //        await _context.SaveChangesAsync(); // зберігаємо зміни в базі даних

        //        return Ok(user);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error updating user");
        //        return StatusCode(500, "An error occurred while updating the user.");
        //    }
        //}
    }
}
