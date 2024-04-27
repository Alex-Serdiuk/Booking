using BookingServer.Models;
using BookingServer.Models.Forms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<User> _userManager;

        public UserController(BookingDbContext context, ILogger<HotelController> logger, UserManager<User> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserForm user)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(id); // Знаходимо користувача за ідентифікатором
                if (existingUser == null)
                {
                    return NotFound(); // Якщо користувач не знайдений, повертаємо 404
                }
                // Оновлюємо дані користувача з відповідними даними з запиту
                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;
                existingUser.Img = user.Img;
                existingUser.Country = user.Country;
                existingUser.City = user.City;
                existingUser.PhoneNumber = user.PhoneNumber;

                

                //existingUser.Password = user.Password; // Наприклад, якщо ви оновлюєте пароль
                                                       // Оновлюємо інші поля користувача за необхідності

                _context.Users.Update(existingUser); // Позначаємо користувача як оновленого в контексті
                await _context.SaveChangesAsync(); // Зберігаємо зміни в базі даних
                if (user.Password.Any() && user.NewPassword.Any())
                {
                    //var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
                    //await _userManager.ResetPasswordAsync(existingUser, token, newPassword: "newPassword");
                    var result = await _userManager.ChangePasswordAsync(existingUser, user.Password, user.NewPassword);
                    if (!result.Succeeded)
                    {
                        throw new Exception();
                    }
                }
                

                return Ok(existingUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                return StatusCode(500, "An error occurred while updating the user.");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // You can handle errors appropriately here
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id); // знаходимо user за ідентифікатором
                if (user == null)
                {
                    return NotFound(); // якщо user не знайдений, повертаємо 404
                }
                _context.Users.Remove(user); // видаляємо user з контексту
                await _context.SaveChangesAsync(); // зберігаємо зміни в базі даних
                return Ok("User has been deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting hotel: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            try
            {
                var userDB = await _context.Users.FindAsync(id);
                if (userDB == null)
                {
                    return NotFound(); // якщо user не знайдений, повертаємо 404
                }
                var user = new UserForm
                {
                    UserName = userDB.UserName,
                    Email = userDB.Email,
                    Img = userDB.Img,
                    Country = userDB.Country,
                    City = userDB.City,
                    PhoneNumber = userDB.PhoneNumber
                };
                
                return Ok(user);
               
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving user: {ex.Message}");
            }
        }
    }
}
