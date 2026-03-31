using Microsoft.AspNetCore.Mvc;
using PetMateAPI.DTOs;
using PetMateAPI.Services;

namespace PetMateAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // POST /api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var (success, error, data) = await _authService.RegisterAsync(dto);

        if (!success)
            return BadRequest(new { message = error });

        return StatusCode(201, new
        {
            message = "Welcome to PetMate! 🐾",
            user = data
        });
    }
}

