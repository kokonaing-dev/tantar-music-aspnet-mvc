using TANTAR_Music.Models.ViewModels;

namespace TANTAR_Music.Services;

public interface IPlaylistService
{
    Task<IEnumerable<PlaylistDetailViewModel>> GetUserPlaylistsAsync(string userId);
    Task<IEnumerable<PlaylistDetailViewModel>> GetPublicPlaylistsAsync();
    Task<PlaylistDetailViewModel?> GetByIdAsync(int id);
    Task<int> CreateAsync(PlaylistViewModel model, string userId);
    Task UpdateAsync(PlaylistViewModel model);
    Task DeleteAsync(int id);
    Task AddSongAsync(int playlistId, int songId);
    Task RemoveSongAsync(int playlistId, int songId);
}
