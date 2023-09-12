using Microsoft.AspNetCore.Mvc;
using TodoApi.Auth;
using TodoApi.Database;
using TodoApi.Models;

namespace TodoApi.Controllers;

[ApiController]
[Route("[controller]")]
[ValidateAntiForgeryToken]
public class AuthController : ControllerBase
{
    private readonly JWTGenerator _jWTGenerator;
    private readonly DBContext _context;

    public AuthController(DBContext context)
    {
        _jWTGenerator = new JWTGenerator("Secret");
        _context = context;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (request.Username == "admin" && request.Password == "admin")
        {
            var token = _jWTGenerator.GenerateToken("1", "admin");
            return Ok(new LoginResponse { Token = token });
        }
        else
        {
            return Unauthorized();
        }
    }

    /// <summary>
    /// Register a user.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    [HttpPost("register")]
    // [SwaggerOperation(Summary = "Get all items", Description = "Get a list of all items.")]
    // [SwaggerResponse(200, "Success", typeof(IEnumerable<Item>))]
    public IActionResult Register([FromBody] User user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        string hashedPassword = Helpers.HashPassword(user.Password!);

        var result = _context.Users.Add(new User
        {
            Username = user.Username,
            Password = hashedPassword,
            Email = user.Email
        });

        return Ok(result.Entity);
    }
}

public class LoginRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}

public class LoginResponse
{
    public string? Token { get; set; }
}

public class RegisterRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
}