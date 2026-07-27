using Microsoft.AspNetCore.Mvc.Rendering;
using TANTAR_Music.Models.Domain;
using TANTAR_Music.Models.ViewModels;
using TANTAR_Music.Repositories;

namespace TANTAR_Music.Services;

public class AlbumService(IAlbumRepository albumRepo, IArtistRepository artistRepo, IFileService fileService) : IAlbumService
{
    public async Task<IEnumerable<AlbumDetailViewModel>> GetAllAsync() =>
        (await albumRepo.GetAllAsync()).Select(ToDetail);

    public async Task<AlbumDetailViewModel?> GetByIdAsync(int id)
    {
        var album = await albumRepo.GetByIdAsync(id);
        return album == null ? null : ToDetail(album);
    }

    public async Task<IEnumerable<AlbumDetailViewModel>> GetRecentAsync(int count = 6) =>
        (await albumRepo.GetRecentAsync(count)).Select(ToDetail);

    public async Task<AlbumViewModel> GetCreateFormAsync() => new() { Artists = await BuildArtistList() };

    public async Task<AlbumViewModel?> GetEditFormAsync(int id)
    {
        var album = await albumRepo.GetByIdAsync(id);
        if (album == null) return null;
        return new AlbumViewModel
        {
            Id = album.Id,
            Title = album.Title,
            ArtistId = album.ArtistId,
            Genre = album.Genre,
            ReleaseDate = album.ReleaseDate,
            ExistingCoverPath = album.CoverImagePath,
            Artists = await BuildArtistList()
        };
    }

    public async Task CreateAsync(AlbumViewModel model)
    {
        var coverPath = model.CoverImage != null ? await fileService.SaveCoverAsync(model.CoverImage) : null;
        await albumRepo.CreateAsync(new Album
        {
            Title = model.Title,
            ArtistId = model.ArtistId,
            Genre = model.Genre,
            ReleaseDate = model.ReleaseDate,
            CoverImagePath = coverPath
        });
    }

    public async Task UpdateAsync(AlbumViewModel model)
    {
        var album = await albumRepo.GetByIdAsync(model.Id) ?? throw new InvalidOperationException("Album not found");
        album.Title = model.Title;
        album.ArtistId = model.ArtistId;
        album.Genre = model.Genre;
        album.ReleaseDate = model.ReleaseDate;

        if (model.CoverImage != null)
        {
            if (album.CoverImagePath != null) fileService.DeleteFile(album.CoverImagePath);
            album.CoverImagePath = await fileService.SaveCoverAsync(model.CoverImage);
        }

        await albumRepo.UpdateAsync(album);
    }

    public async Task DeleteAsync(int id)
    {
        var album = await albumRepo.GetByIdAsync(id);
        if (album != null)
        {
            if (album.CoverImagePath != null) fileService.DeleteFile(album.CoverImagePath);
            await albumRepo.DeleteAsync(id);
        }
    }

    private async Task<List<SelectListItem>> BuildArtistList() =>
        (await artistRepo.GetAllAsync()).Select(a => new SelectListItem(a.Name, a.Id.ToString())).ToList();

    private static AlbumDetailViewModel ToDetail(Album a) => new()
    {
        Id = a.Id,
        Title = a.Title,
        ArtistName = a.Artist.Name,
        ArtistId = a.ArtistId,
        CoverImagePath = a.CoverImagePath,
        ReleaseDate = a.ReleaseDate,
        Genre = a.Genre,
        Songs = a.Songs.Select(s => new SongListItemViewModel
        {
            Id = s.Id,
            Title = s.Title,
            ArtistName = s.Artist?.Name ?? a.Artist?.Name ?? string.Empty,
            AlbumTitle = a.Title,
            CoverImagePath = s.CoverImagePath ?? a.CoverImagePath,
            FilePath = s.FilePath,
            DurationSeconds = s.DurationSeconds,
            Genre = s.Genre,
            PlayCount = s.PlayCount
        }).ToList()
    };
}
