using BookingServer.Models.Forms;

namespace BookingServer.Services
{
    public interface IAuthService
    {
        string GenerateTokenString(LoginForm user);
        Task<bool> Login(LoginForm user);
        Task<bool> RegisterUser(RegisterForm user);
    }
}