using System.ComponentModel.DataAnnotations;

namespace TANTAR_Music.Models.ViewModels;

public class ArtistViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Bio { get; set; }

    [Display(Name = "Profile Image")]
    public IFormFile? Image { get; set; }

    public string? ExistingImagePath { get; set; }
}

public class ArtistDetailViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? ImagePath { get; set; }
    public List<AlbumDetailViewModel> Albums { get; set; } = [];
    public List<SongListItemViewModel> Songs { get; set; } = [];
}
