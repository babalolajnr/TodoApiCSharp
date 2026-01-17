using TodoApi.Controllers; // For DTOs

namespace TodoApi.Services;

public interface IAuthService
{
    Task<(string Token, string RefreshToken)?> Login(LoginRequest request);
    Task<string?> Register(RegisterRequest request);
    Task<(string Token, string RefreshToken)?> RefreshToken(string accessToken, string refreshToken);
}
