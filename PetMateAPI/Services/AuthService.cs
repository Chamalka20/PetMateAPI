using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PetMateAPI.DTOs;
using PetMateAPI.Models;

namespace PetMateAPI.Services;

public interface IAuthService
{
    Task<(bool Success, string? Error, RegisterResponseDto? Data)> RegisterAsync(RegisterDto dto);
}

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _config;

    public AuthService(UserManager<AppUser> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    public async Task<(bool Success, string? Error, RegisterResponseDto? Data)> RegisterAsync(RegisterDto dto)
    {
        // 1. Check if email already exists
        var existing = await _userManager.FindByEmailAsync(dto.Email);
        if (existing != null)
            return (false, "This email is already registered.", null);

        // 2. Create user object
        var user = new AppUser
        {
            FullName = dto.FullName,
            Email = dto.Email,
            UserName = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            ProfilePhotoUrl = dto.ProfilePhotoUrl,
            CreatedAt = DateTime.UtcNow
        };

        // 3. Save to database with hashed password
        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var error = result.Errors.FirstOrDefault()?.Description;
            return (false, error, null);
        }

        // 4. Generate JWT token
        var token = GenerateToken(user);

        // 5. Return response
        return (true, null, new RegisterResponseDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber,
            ProfilePhotoUrl = user.ProfilePhotoUrl,
            Token = token,
            CreatedAt = user.CreatedAt
        });
    }

    private string GenerateToken(AppUser user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email,          user.Email!),
            new Claim(ClaimTypes.Name,           user.FullName),
            new Claim("phoneNumber",             user.PhoneNumber ?? ""),
            new Claim("profilePhoto",            user.ProfilePhotoUrl ?? "")
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
