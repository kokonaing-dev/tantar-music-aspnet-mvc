using TANTAR_Music.Models.Domain;
using TANTAR_Music.Models.ViewModels;
using TANTAR_Music.Repositories;

namespace TANTAR_Music.Services;

public class PlaylistService(IPlaylistRepository playlistRepo, IFileService fileService) : IPlaylistService
{
    public async Task<IEnumerable<PlaylistDetailViewModel>> GetUserPlaylistsAsync(string userId) =>
        (await playlistRepo.GetByUserAsync(userId)).Select(ToDetail);

    public async Task<IEnumerable<PlaylistDetailViewModel>> GetPublicPlaylistsAsync() =>
        (await playlistRepo.GetPublicAsync()).Select(ToDetail);

    public async Task<PlaylistDetailViewModel?> GetByIdAsync(int id)
    {
        var playlist = await playlistRepo.GetByIdAsync(id);
        return playlist == null ? null : ToDetail(playlist);
    }

    public async Task<int> CreateAsync(PlaylistViewModel model, string userId)
    {
        var coverPath = model.CoverImage != null ? await fileService.SaveCoverAsync(model.CoverImage) : null;
        var playlist = await playlistRepo.CreateAsync(new Playlist
        {
            Name = model.Name,
            Description = model.Description,
            IsPublic = model.IsPublic,
            CoverImagePath = coverPath,
            UserId = userId
        });
        return playlist.Id;
    }

    public async Task UpdateAsync(PlaylistViewModel model)
    {
        var playlist = await playlistRepo.GetByIdAsync(model.Id) ?? throw new InvalidOperationException("Playlist not found");
        playlist.Name = model.Name;
        playlist.Description = model.Description;
        playlist.IsPublic = model.IsPublic;

        if (model.CoverImage != null)
        {
            if (playlist.CoverImagePath != null) fileService.DeleteFile(playlist.CoverImagePath);
            playlist.CoverImagePath = await fileService.SaveCoverAsync(model.CoverImage);
        }

        await playlistRepo.UpdateAsync(playlist);
    }

    public async Task DeleteAsync(int id)
    {
        var playlist = await playlistRepo.GetByIdAsync(id);
        if (playlist != null)
        {
            if (playlist.CoverImagePath != null) fileService.DeleteFile(playlist.CoverImagePath);
            await playlistRepo.DeleteAsync(id);
        }
    }

    public Task AddSongAsync(int playlistId, int songId) => playlistRepo.AddSongAsync(playlistId, songId);

    public Task RemoveSongAsync(int playlistId, int songId) => playlistRepo.RemoveSongAsync(playlistId, songId);

    private static PlaylistDetailViewModel ToDetail(Playlist p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        IsPublic = p.IsPublic,
        CoverImagePath = p.CoverImagePath,
        OwnerName = p.User?.DisplayName ?? p.User?.Email ?? "Unknown",
        OwnerId = p.UserId,
        Songs = p.PlaylistSongs.OrderBy(ps => ps.Order).Select(ps => new SongListItemViewModel
        {
            Id = ps.Song.Id,
            Title = ps.Song.Title,
            ArtistName = ps.Song.Artist?.Name ?? "",
            AlbumTitle = ps.Song.Album?.Title,
            CoverImagePath = ps.Song.CoverImagePath ?? ps.Song.Album?.CoverImagePath,
            FilePath = ps.Song.FilePath,
            DurationSeconds = ps.Song.DurationSeconds,
            Genre = ps.Song.Genre,
            PlayCount = ps.Song.PlayCount
        }).ToList()
    };
}
