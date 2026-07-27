using TANTAR_Music.Models.Domain;

namespace TANTAR_Music.Repositories;

public interface IAlbumRepository
{
    Task<IEnumerable<Album>> GetAllAsync();
    Task<Album?> GetByIdAsync(int id);
    Task<IEnumerable<Album>> GetByArtistAsync(int artistId);
    Task<IEnumerable<Album>> GetRecentAsync(int count);
    Task<Album> CreateAsync(Album album);
    Task<Album> UpdateAsync(Album album);
    Task DeleteAsync(int id);
}
