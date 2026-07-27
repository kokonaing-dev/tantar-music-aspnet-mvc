using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TANTAR_Music.Models.ViewModels;

public class AlbumViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, Display(Name = "Artist")]
    public int ArtistId { get; set; }

    public string? Genre { get; set; }

    [Display(Name = "Release Date")]
    [DataType(DataType.Date)]
    public DateTime ReleaseDate { get; set; } = DateTime.Today;

    [Display(Name = "Cover Image")]
    public IFormFile? CoverImage { get; set; }

    public string? ExistingCoverPath { get; set; }

    public List<SelectListItem> Artists { get; set; } = [];
}

public class AlbumDetailViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ArtistName { get; set; } = string.Empty;
    public int ArtistId { get; set; }
    public string? CoverImagePath { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string? Genre { get; set; }
    public List<SongListItemViewModel> Songs { get; set; } = [];
}
