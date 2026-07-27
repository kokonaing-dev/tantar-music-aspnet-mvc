namespace TANTAR_Music.Models.Domain;

public class Album
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ArtistId { get; set; }
    public Artist Artist { get; set; } = null!;
    public string? CoverImagePath { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string? Genre { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Song> Songs { get; set; } = [];
}
