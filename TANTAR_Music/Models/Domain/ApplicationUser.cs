using Microsoft.AspNetCore.Identity;

namespace TANTAR_Music.Models.Domain;

public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
    public string? ProfileImagePath { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Playlist> Playlists { get; set; } = [];
}
