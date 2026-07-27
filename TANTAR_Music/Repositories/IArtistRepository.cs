using TANTAR_Music.Models.Domain;

namespace TANTAR_Music.Repositories;

public interface IArtistRepository
{
    Task<IEnumerable<Artist>> GetAllAsync();
    Task<Artist?> GetByIdAsync(int id);
    Task<IEnumerable<Artist>> GetFeaturedAsync(int count);
    Task<Artist> CreateAsync(Artist artist);
    Task<Artist> UpdateAsync(Artist artist);
    Task DeleteAsync(int id);
}
