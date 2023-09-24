using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
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

    public AuthController(DBContext context, ILogger<AuthController> logger)
    {
        _jWTGenerator = new JWTGenerator("Secret");
        _context = context;
        _logger = logger;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (request.Email == "admin" && request.Password == "admin")
        {
            var token = _jWTGenerator.GenerateToken("1", "admin");
            return Ok(new LoginResponse { Token = token });
        }
        else
        {
            return Unauthorized();
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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
            throw new Exception("Error registering user");
        }
    }
}

public class LoginRequest
{
    public string? Email { get; set; }
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