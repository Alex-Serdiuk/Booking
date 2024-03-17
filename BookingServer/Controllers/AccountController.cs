using BookingServer.Models;
using BookingServer.Models.Forms;
using BookingServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly BookingDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole<int>> roleManager, BookingDbContext context, IAuthService authService)
        {
            _authService = authService;
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser(RegisterForm user)
        {
            if (await _authService.RegisterUser(user))
            {
                return Ok("Successfuly done");
            }
            return BadRequest("Something went worng");
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginForm userForm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                //var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userForm.UserName);
                var user = await _userManager.FindByNameAsync(userForm.UserName);

                //if (await _authService.Login(userForm))
                //{
                //    return NotFound(new { message = "User not found or wrong password!" });
                //}
                if (user == null)
                {
                    return NotFound(new { message = "User not found!" });
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName, userForm.Password, true, false);

                if (!result.Succeeded)
                {
                    return NotFound(new { message = "Wrong password or username!" }); ;
                }

                var tokenString = _authService.GenerateTokenString(userForm);
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.Now.AddMinutes(60) // Настройте срок действия куки
                };
                Response.Cookies.Append("access_token", tokenString, cookieOptions);

                var IsAdmin = await _userManager.IsInRoleAsync(user, "Admin");

                return Ok(new
                {
                    //AccessToken = tokenString,
                    Details = new { user.Id, user.UserName, user.Email},
                    IsAdmin = new { IsAdmin }
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }

        }

        //[HttpPost("Login")]
        //public async Task<IActionResult> Login(LoginForm user)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest();
        //    }
        //    if (await _authService.Login(user))
        //    {
        //        var tokenString = _authService.GenerateTokenString(user);
        //        return Ok(tokenString);
        //    }
        //    return BadRequest();
        //}

        [HttpPost("assignRoleToAdmin")]
        public async Task<IActionResult> AssignRoleToAdmin()
        {
            // Знайдіть користувача з ім'ям "admin"
            var adminUser = await _userManager.FindByNameAsync("admin");

            if (adminUser == null)
            {
                // Якщо користувач не знайдений, поверніть помилку або відповідну відповідь
                return NotFound("Користувач admin не знайдений");
            }

            // Перевірте чи існує роль "Admin"
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                // Якщо роль не існує, створіть її
                await _roleManager.CreateAsync(new IdentityRole<int>("Admin"));
            }

            // Додайте роль "Admin" користувачеві
            await _userManager.AddToRoleAsync(adminUser, "Admin");

            return Ok("Роль Admin надано користувачеві admin");
        }
    }
}
