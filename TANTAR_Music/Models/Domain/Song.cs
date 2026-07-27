namespace TANTAR_Music.Models.Domain;

public class Song
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ArtistId { get; set; }
    public Artist Artist { get; set; } = null!;
    public int? AlbumId { get; set; }
    public Album? Album { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string? CoverImagePath { get; set; }
    public int DurationSeconds { get; set; }
    public string? Genre { get; set; }
    public int PlayCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<PlaylistSong> PlaylistSongs { get; set; } = [];
}
