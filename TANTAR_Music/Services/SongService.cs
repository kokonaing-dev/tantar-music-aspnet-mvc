using Microsoft.AspNetCore.Mvc.Rendering;
using TANTAR_Music.Models.Domain;
using TANTAR_Music.Models.ViewModels;
using TANTAR_Music.Repositories;

namespace TANTAR_Music.Services;

public class SongService(ISongRepository songRepo, IArtistRepository artistRepo, IAlbumRepository albumRepo, IFileService fileService) : ISongService
{
    public async Task<IEnumerable<SongListItemViewModel>> GetAllAsync() =>
        (await songRepo.GetAllAsync()).Select(ToListItem);

    public async Task<SongListItemViewModel?> GetByIdAsync(int id)
    {
        var song = await songRepo.GetByIdAsync(id);
        return song == null ? null : ToListItem(song);
    }

    public async Task<IEnumerable<SongListItemViewModel>> GetTopAsync(int count = 10) =>
        (await songRepo.GetTopAsync(count)).Select(ToListItem);

    public async Task<IEnumerable<SongListItemViewModel>> GetRecentAsync(int count = 10) =>
        (await songRepo.GetRecentAsync(count)).Select(ToListItem);

    public async Task<IEnumerable<SongListItemViewModel>> SearchAsync(string query) =>
        (await songRepo.SearchAsync(query)).Select(ToListItem);

    public async Task<SongViewModel> GetCreateFormAsync() => new()
    {
        Artists = await BuildArtistList(),
        Albums = await BuildAlbumList()
    };

    public async Task<SongViewModel?> GetEditFormAsync(int id)
    {
        var song = await songRepo.GetByIdAsync(id);
        if (song == null) return null;
        return new SongViewModel
        {
            Id = song.Id,
            Title = song.Title,
            ArtistId = song.ArtistId,
            AlbumId = song.AlbumId,
            Genre = song.Genre,
            ExistingFilePath = song.FilePath,
            ExistingCoverPath = song.CoverImagePath,
            Artists = await BuildArtistList(),
            Albums = await BuildAlbumList()
        };
    }

    public async Task CreateAsync(SongViewModel model)
    {
        var filePath = await fileService.SaveAudioAsync(model.AudioFile!);
        var coverPath = model.CoverImage != null ? await fileService.SaveCoverAsync(model.CoverImage) : null;
        var duration = await fileService.GetAudioDurationAsync(filePath);

        await songRepo.CreateAsync(new Song
        {
            Title = model.Title,
            ArtistId = model.ArtistId,
            AlbumId = model.AlbumId,
            Genre = model.Genre,
            FilePath = filePath,
            CoverImagePath = coverPath,
            DurationSeconds = duration
        });
    }

    public async Task UpdateAsync(SongViewModel model)
    {
        var song = await songRepo.GetByIdAsync(model.Id) ?? throw new InvalidOperationException("Song not found");

        song.Title = model.Title;
        song.ArtistId = model.ArtistId;
        song.AlbumId = model.AlbumId;
        song.Genre = model.Genre;

        if (model.AudioFile != null)
        {
            fileService.DeleteFile(song.FilePath);
            song.FilePath = await fileService.SaveAudioAsync(model.AudioFile);
            song.DurationSeconds = await fileService.GetAudioDurationAsync(song.FilePath);
        }

        if (model.CoverImage != null)
        {
            if (song.CoverImagePath != null) fileService.DeleteFile(song.CoverImagePath);
            song.CoverImagePath = await fileService.SaveCoverAsync(model.CoverImage);
        }

        await songRepo.UpdateAsync(song);
    }

    public async Task DeleteAsync(int id)
    {
        var song = await songRepo.GetByIdAsync(id);
        if (song != null)
        {
            fileService.DeleteFile(song.FilePath);
            if (song.CoverImagePath != null) fileService.DeleteFile(song.CoverImagePath);
            await songRepo.DeleteAsync(id);
        }
    }

    public Task IncrementPlayCountAsync(int id) => songRepo.IncrementPlayCountAsync(id);

    private async Task<List<SelectListItem>> BuildArtistList() =>
        (await artistRepo.GetAllAsync()).Select(a => new SelectListItem(a.Name, a.Id.ToString())).ToList();

    private async Task<List<SelectListItem>> BuildAlbumList() =>
        (await albumRepo.GetAllAsync()).Select(a => new SelectListItem($"{a.Title} – {a.Artist.Name}", a.Id.ToString())).ToList();

    private static SongListItemViewModel ToListItem(Song s) => new()
    {
        Id = s.Id,
        Title = s.Title,
        ArtistName = s.Artist.Name,
        AlbumTitle = s.Album?.Title,
        CoverImagePath = s.CoverImagePath ?? s.Album?.CoverImagePath,
        FilePath = s.FilePath,
        DurationSeconds = s.DurationSeconds,
        Genre = s.Genre,
        PlayCount = s.PlayCount
    };
}
