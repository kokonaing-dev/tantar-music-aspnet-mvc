using TANTAR_Music.Models.Domain;

namespace TANTAR_Music.Repositories;

public interface ISongRepository
{
    Task<IEnumerable<Song>> GetAllAsync();
    Task<Song?> GetByIdAsync(int id);
    Task<IEnumerable<Song>> GetByArtistAsync(int artistId);
    Task<IEnumerable<Song>> GetByAlbumAsync(int albumId);
    Task<IEnumerable<Song>> GetTopAsync(int count);
    Task<IEnumerable<Song>> GetRecentAsync(int count);
    Task<IEnumerable<Song>> SearchAsync(string query);
    Task<Song> CreateAsync(Song song);
    Task<Song> UpdateAsync(Song song);
    Task DeleteAsync(int id);
    Task IncrementPlayCountAsync(int id);
}
