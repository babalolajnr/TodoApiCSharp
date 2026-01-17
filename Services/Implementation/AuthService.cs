using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using TodoApi.Auth;
using TodoApi.Controllers; // For DTOs
using TodoApi.Database;
using TodoApi.Models;

namespace TodoApi.Services.Implementation;

public class AuthService : IAuthService
{
    private readonly DBContext _context;
    private readonly JWTGenerator _jwtGenerator;
    private readonly IConfiguration _configuration;

    public AuthService(DBContext context, JWTGenerator jwtGenerator, IConfiguration configuration)
    {
        _context = context;
        _jwtGenerator = jwtGenerator;
        _configuration = configuration;
    }

    public async Task<(string Token, string RefreshToken)?> Login(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !Helpers.VerifyPassword(request.Password!, user.Password!))
        {
            return null;
        }

        var token = _jwtGenerator.GenerateToken(user.Id.ToString(), user.Name!);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Refresh token valid for 7 days

        await _context.SaveChangesAsync();

        return (token, refreshToken);
    }

    public async Task<string?> Register(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return null; // or throw exception user already exists
        }

        string hashedPassword = Helpers.HashPassword(request.Password!);

        var user = new User
        {
            Name = request.Name,
            Password = hashedPassword,
            Email = request.Email
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user.Email;
    }

    public async Task<(string Token, string RefreshToken)?> RefreshToken(string accessToken, string refreshToken)
    {
        // Here we could validate the access token structure even if expired, 
        // but typically the client sends the expired access token and the refresh token.
        // We primarily need to validate the refresh token against the database.
        
        // Find user by refresh token? 
        // Or strictly we should rely on the claims from expired access token to find the user ID.
        // However, extracting claims from expired token is possible.
        
        // For simplicity, let's assume the client might send user ID or we find by token.
        // BUT, since one user has one refresh token, we can search by refresh token.
        
        var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return null;
        }

        // Generate new tokens
        var newToken = _jwtGenerator.GenerateToken(user.Id.ToString(), user.Name!);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); 
        
        await _context.SaveChangesAsync();

        return (newToken, newRefreshToken);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
