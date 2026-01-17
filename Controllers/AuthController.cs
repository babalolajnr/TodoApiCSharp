using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Services;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _authService.Login(request);

            if (result == null)
            {
                return Unauthorized();
            }

            return Ok(new LoginResponse { Token = result.Value.Token, RefreshToken = result.Value.RefreshToken });
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
            var result = await _authService.Register(request);

            if (result == null)
            {
                return Conflict(new { message = "User already exists" });
            }

            return CreatedAtAction(nameof(Register), new { email = result }, request);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (request is null || string.IsNullOrEmpty(request.AccessToken) || string.IsNullOrEmpty(request.RefreshToken))
        {
            return BadRequest("Invalid client request");
        }

        var result = await _authService.RefreshToken(request.AccessToken, request.RefreshToken);

        if (result == null)
        {
            return BadRequest("Invalid token");
        }

        return Ok(new LoginResponse { Token = result.Value.Token, RefreshToken = result.Value.RefreshToken });
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
    public string? RefreshToken { get; set; }
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

public class RefreshTokenRequest
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}