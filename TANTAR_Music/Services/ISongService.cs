using TANTAR_Music.Models.ViewModels;

namespace TANTAR_Music.Services;

public interface ISongService
{
    Task<IEnumerable<SongListItemViewModel>> GetAllAsync();
    Task<SongListItemViewModel?> GetByIdAsync(int id);
    Task<IEnumerable<SongListItemViewModel>> GetTopAsync(int count = 10);
    Task<IEnumerable<SongListItemViewModel>> GetRecentAsync(int count = 10);
    Task<IEnumerable<SongListItemViewModel>> SearchAsync(string query);
    Task<SongViewModel> GetCreateFormAsync();
    Task<SongViewModel?> GetEditFormAsync(int id);
    Task CreateAsync(SongViewModel model);
    Task UpdateAsync(SongViewModel model);
    Task DeleteAsync(int id);
    Task IncrementPlayCountAsync(int id);
}
