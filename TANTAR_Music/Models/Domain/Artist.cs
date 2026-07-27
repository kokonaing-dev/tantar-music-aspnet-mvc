namespace TANTAR_Music.Models.Domain;

public class Artist
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? ImagePath { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Song> Songs { get; set; } = [];
    public ICollection<Album> Albums { get; set; } = [];
}
