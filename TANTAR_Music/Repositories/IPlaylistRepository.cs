using TANTAR_Music.Models.Domain;

namespace TANTAR_Music.Repositories;

public interface IPlaylistRepository
{
    Task<IEnumerable<Playlist>> GetByUserAsync(string userId);
    Task<IEnumerable<Playlist>> GetPublicAsync();
    Task<Playlist?> GetByIdAsync(int id);
    Task<Playlist> CreateAsync(Playlist playlist);
    Task<Playlist> UpdateAsync(Playlist playlist);
    Task DeleteAsync(int id);
    Task AddSongAsync(int playlistId, int songId);
    Task RemoveSongAsync(int playlistId, int songId);
}
