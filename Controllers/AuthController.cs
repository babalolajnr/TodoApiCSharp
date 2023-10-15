using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TodoApi.Auth;
using TodoApi.Database;
using TodoApi.Models;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
// [ValidateAntiForgeryToken]
public class AuthController : ControllerBase
{
    private readonly JWTGenerator _jWTGenerator;
    private readonly DBContext _context;
    private readonly ILogger<AuthController> _logger;
    private readonly IConfiguration _configuration;

    public AuthController(DBContext context, ILogger<AuthController> logger, IConfiguration configuration, JWTGenerator jWTGenerator)
    {
        _jWTGenerator = jWTGenerator;
        _context = context;
        _logger = logger;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            // Check if user exists
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return Unauthorized();
            }

            // Check if password is correct
            if (!Helpers.VerifyPassword(request.Password!, user.Password!))
            {
                return Unauthorized();
            }

            // Generate token
            var token = _jWTGenerator.GenerateToken(user.Id.ToString(), user.Name!);

            return Ok(new LoginResponse { Token = token });
        }
        catch (Exception e)
        {
            _logger.LogError("message: {exception}", e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            string hashedPassword = Helpers.HashPassword(request.Password!);

            _context.Users.Add(new User
            {
                Name = request.Name,
                Password = hashedPassword,
                Email = request.Email
            });

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Register), new { email = request.Email }, request);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    public string? Password { get; set; }
}

public class LoginResponse
{
    public string? Token { get; set; }
}

public class RegisterRequest
{
    [Required]
    [MaxLength(50)]
    public string? Name { get; set; }

    [Required]
    [MinLength(8)]
    public string? Password { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }
}