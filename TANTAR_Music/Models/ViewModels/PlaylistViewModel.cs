using System.ComponentModel.DataAnnotations;

namespace TANTAR_Music.Models.ViewModels;

public class PlaylistViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Display(Name = "Public")]
    public bool IsPublic { get; set; }

    [Display(Name = "Cover Image")]
    public IFormFile? CoverImage { get; set; }

    public string? ExistingCoverPath { get; set; }
}

public class PlaylistDetailViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    public string? CoverImagePath { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public List<SongListItemViewModel> Songs { get; set; } = [];
}
