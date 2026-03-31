using Microsoft.AspNetCore.Identity;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace PetMateAPI.Models;
[Table("profiles")]  
public class AppUser : BaseModel
{
    [PrimaryKey("id", false)]
    public string Id { get; set; } = string.Empty;

    [Column("full_name")]
    public string FullName { get; set; } = string.Empty;

    [Column("phone_number")]
    public string? PhoneNumber { get; set; }

    [Column("profile_photo_url")]
    public string? ProfilePhotoUrl { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
