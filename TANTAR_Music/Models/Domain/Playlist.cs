namespace TANTAR_Music.Models.Domain;

public class Playlist
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public bool IsPublic { get; set; }
    public string? CoverImagePath { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<PlaylistSong> PlaylistSongs { get; set; } = [];
}
