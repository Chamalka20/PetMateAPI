using Microsoft.AspNetCore.Identity;

namespace PetMateAPI.Models;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? ProfilePhotoUrl { get; set; }
    public bool IsGoogleUser { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
