using TANTAR_Music.Models.ViewModels;

namespace TANTAR_Music.Services;

public interface IAlbumService
{
    Task<IEnumerable<AlbumDetailViewModel>> GetAllAsync();
    Task<AlbumDetailViewModel?> GetByIdAsync(int id);
    Task<IEnumerable<AlbumDetailViewModel>> GetRecentAsync(int count = 6);
    Task<AlbumViewModel> GetCreateFormAsync();
    Task<AlbumViewModel?> GetEditFormAsync(int id);
    Task CreateAsync(AlbumViewModel model);
    Task UpdateAsync(AlbumViewModel model);
    Task DeleteAsync(int id);
}
