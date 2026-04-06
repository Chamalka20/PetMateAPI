using Microsoft.AspNetCore.Mvc;
using PetMateAPI.DTOs;
using PetMateAPI.Services;

namespace PetMateAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>Register a new PetMate account</summary>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ErrorResponseDto
            {
                Message = "Validation failed.",
                Errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            });

        var (success, error, errors, data) = await _authService.RegisterAsync(dto);

        if (!success)
            return BadRequest(new ErrorResponseDto
            {
                Message = error!,
                Errors = errors
            });

        return StatusCode(201, new
        {
            message = "Welcome to PetMate! Your account has been created. 🐾",
            user = data
        });
    }

    /// <summary>Sign in with email and password</summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ErrorResponseDto
            {
                Message = "Validation failed.",
                Errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            });

        var (success, error, data) = await _authService.LoginAsync(dto);

        if (!success)
            return Unauthorized(new ErrorResponseDto { Message = error! });

        return Ok(new
        {
            message = "Welcome back to PetMate! 🐾",
            user = data
        });
    }

    /// <summary>Sign in with Google via Firebase</summary>
    [HttpPost("google")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ErrorResponseDto
            {
                Message = "Firebase token is required."
            });

        var (success, error, data) = await _authService.GoogleSignInAsync(dto);

        if (!success)
            return BadRequest(new ErrorResponseDto { Message = error! });

        return Ok(new
        {
            message = "Welcome to PetMate! 🐾",
            user = data
        });
    }
}