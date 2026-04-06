using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PetMateAPI.DTOs;
using PetMateAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PetMateAPI.Services;

public interface IAuthService
{

    Task<(bool Success, string? Error, List<string>? Errors, AuthResponseDto? Data)>
        RegisterAsync(RegisterDto dto);

    Task<(bool Success, string? Error, AuthResponseDto? Data)>
        LoginAsync(LoginDto dto);

    Task<(bool Success, string? Error, AuthResponseDto? Data)>
        GoogleSignInAsync(GoogleSignInDto dto);
}

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<AppUser> userManager,
        IConfiguration config,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _config = config;
        _logger = logger;
    }

    // ── Register ──────────────────────────────────
    public async Task<(bool Success, string? Error, List<string>? Errors, AuthResponseDto? Data)>
        RegisterAsync(RegisterDto dto)
    {
        // 1. Check email already exists
        var existing = await _userManager.FindByEmailAsync(dto.Email);
        if (existing != null)
            return (false, "This email is already registered.", null, null);

        // 2. Create user
        var user = new AppUser
        {
            FullName = dto.FullName,
            Email = dto.Email,
            UserName = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            ProfilePhotoUrl = dto.ProfilePhotoUrl,
            IsGoogleUser = false,
            CreatedAt = DateTime.UtcNow
        };

        // 3. Save with hashed password
        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return (false, "Registration failed.", errors, null);
        }

        _logger.LogInformation("New user registered: {Email}", dto.Email);

        return (true, null, null, BuildResponse(user));
    }

    // ── Login ─────────────────────────────────────
    public async Task<(bool Success, string? Error, AuthResponseDto? Data)>
        LoginAsync(LoginDto dto)
    {
        // 1. Find user
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return (false, "Invalid email or password.", null);

        // 2. Check if Google user trying to use password
        if (user.IsGoogleUser)
            return (false, "This account uses Google Sign-In. Please sign in with Google.", null);

        // 3. Verify password
        var validPassword = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!validPassword)
            return (false, "Invalid email or password.", null);

        _logger.LogInformation("User logged in: {Email}", dto.Email);

        return (true, null, BuildResponse(user));
    }

    // ── Google Sign-In ────────────────────────────
    public async Task<(bool Success, string? Error, AuthResponseDto? Data)>
        GoogleSignInAsync(GoogleSignInDto dto)
    {
        try
        {
            // 1. Verify Firebase token
            var decodedToken = await FirebaseAuth.DefaultInstance
                .VerifyIdTokenAsync(dto.FirebaseToken);

            // 2. Extract user info from token
            var email = decodedToken.Claims["email"].ToString()!;
            var name = decodedToken.Claims.ContainsKey("name")
                        ? decodedToken.Claims["name"].ToString()!
                        : email;
            var photo = decodedToken.Claims.ContainsKey("picture")
                        ? decodedToken.Claims["picture"].ToString()
                        : null;

            // 3. Find or create user
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                // First time Google sign-in → create account
                user = new AppUser
                {
                    FullName = name,
                    Email = email,
                    UserName = email,
                    ProfilePhotoUrl = photo,
                    IsGoogleUser = true,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    var error = result.Errors.FirstOrDefault()?.Description;
                    return (false, error, null);
                }

                _logger.LogInformation("New Google user: {Email}", email);
            }
            else
            {
                // Returning Google user → update photo in case it changed
                user.ProfilePhotoUrl = photo;
                await _userManager.UpdateAsync(user);
            }

            return (true, null, BuildResponse(user));
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogWarning("Invalid Firebase token: {Message}", ex.Message);
            return (false, "Invalid or expired Google token. Please sign in again.", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Google sign-in error");
            return (false, "Something went wrong. Please try again.", null);
        }
    }

    // ── Build response DTO ────────────────────────
    private AuthResponseDto BuildResponse(AppUser user) => new()
    {
        Id = user.Id,
        FullName = user.FullName,
        Email = user.Email!,
        PhoneNumber = user.PhoneNumber,
        ProfilePhotoUrl = user.ProfilePhotoUrl,
        IsGoogleUser = user.IsGoogleUser,
        Token = GenerateToken(user),
        CreatedAt = user.CreatedAt
    };

    // ── Generate JWT ──────────────────────────────
    private string GenerateToken(AppUser user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(
            key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email,          user.Email!),
            new Claim(ClaimTypes.Name,           user.FullName),
            new Claim("phoneNumber",             user.PhoneNumber ?? ""),
            new Claim("profilePhoto",            user.ProfilePhotoUrl ?? ""),
            new Claim("isGoogleUser",            user.IsGoogleUser.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}