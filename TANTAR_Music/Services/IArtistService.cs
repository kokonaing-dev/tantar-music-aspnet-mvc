using TANTAR_Music.Models.ViewModels;

namespace TANTAR_Music.Services;

public interface IArtistService
{
    Task<IEnumerable<ArtistDetailViewModel>> GetAllAsync();
    Task<ArtistDetailViewModel?> GetByIdAsync(int id);
    Task<IEnumerable<ArtistDetailViewModel>> GetFeaturedAsync(int count = 6);
    Task<ArtistViewModel> GetCreateFormAsync();
    Task<ArtistViewModel?> GetEditFormAsync(int id);
    Task CreateAsync(ArtistViewModel model);
    Task UpdateAsync(ArtistViewModel model);
    Task DeleteAsync(int id);
}
