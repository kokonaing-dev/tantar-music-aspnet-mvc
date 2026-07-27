using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TANTAR_Music.Models.ViewModels;

public class SongViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, Display(Name = "Artist")]
    public int ArtistId { get; set; }

    [Display(Name = "Album")]
    public int? AlbumId { get; set; }

    public string? Genre { get; set; }

    [Display(Name = "Audio File")]
    public IFormFile? AudioFile { get; set; }

    [Display(Name = "Cover Image")]
    public IFormFile? CoverImage { get; set; }

    public string? ExistingFilePath { get; set; }
    public string? ExistingCoverPath { get; set; }

    public List<SelectListItem> Artists { get; set; } = [];
    public List<SelectListItem> Albums { get; set; } = [];
}

public class SongListItemViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ArtistName { get; set; } = string.Empty;
    public string? AlbumTitle { get; set; }
    public string? CoverImagePath { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
    public string? Genre { get; set; }
    public int PlayCount { get; set; }

    public string FormattedDuration =>
        DurationSeconds >= 3600
            ? TimeSpan.FromSeconds(DurationSeconds).ToString(@"h\:mm\:ss")
            : TimeSpan.FromSeconds(DurationSeconds).ToString(@"m\:ss");
}
