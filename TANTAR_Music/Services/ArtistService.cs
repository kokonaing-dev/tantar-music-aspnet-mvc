using TANTAR_Music.Models.Domain;
using TANTAR_Music.Models.ViewModels;
using TANTAR_Music.Repositories;

namespace TANTAR_Music.Services;

public class ArtistService(IArtistRepository artistRepo, IFileService fileService) : IArtistService
{
    public async Task<IEnumerable<ArtistDetailViewModel>> GetAllAsync() =>
        (await artistRepo.GetAllAsync()).Select(ToDetail);

    public async Task<ArtistDetailViewModel?> GetByIdAsync(int id)
    {
        var artist = await artistRepo.GetByIdAsync(id);
        return artist == null ? null : ToDetail(artist);
    }

    public async Task<IEnumerable<ArtistDetailViewModel>> GetFeaturedAsync(int count = 6) =>
        (await artistRepo.GetFeaturedAsync(count)).Select(ToDetail);

    public Task<ArtistViewModel> GetCreateFormAsync() => Task.FromResult(new ArtistViewModel());

    public async Task<ArtistViewModel?> GetEditFormAsync(int id)
    {
        var artist = await artistRepo.GetByIdAsync(id);
        if (artist == null) return null;
        return new ArtistViewModel
        {
            Id = artist.Id,
            Name = artist.Name,
            Bio = artist.Bio,
            ExistingImagePath = artist.ImagePath
        };
    }

    public async Task CreateAsync(ArtistViewModel model)
    {
        var imagePath = model.Image != null ? await fileService.SaveCoverAsync(model.Image, "profiles") : null;
        await artistRepo.CreateAsync(new Artist { Name = model.Name, Bio = model.Bio, ImagePath = imagePath });
    }

    public async Task UpdateAsync(ArtistViewModel model)
    {
        var artist = await artistRepo.GetByIdAsync(model.Id) ?? throw new InvalidOperationException("Artist not found");
        artist.Name = model.Name;
        artist.Bio = model.Bio;

        if (model.Image != null)
        {
            if (artist.ImagePath != null) fileService.DeleteFile(artist.ImagePath);
            artist.ImagePath = await fileService.SaveCoverAsync(model.Image, "profiles");
        }

        await artistRepo.UpdateAsync(artist);
    }

    public async Task DeleteAsync(int id)
    {
        var artist = await artistRepo.GetByIdAsync(id);
        if (artist != null)
        {
            if (artist.ImagePath != null) fileService.DeleteFile(artist.ImagePath);
            await artistRepo.DeleteAsync(id);
        }
    }

    private static ArtistDetailViewModel ToDetail(Artist a) => new()
    {
        Id = a.Id,
        Name = a.Name,
        Bio = a.Bio,
        ImagePath = a.ImagePath,
        Albums = a.Albums.Select(al => new AlbumDetailViewModel
        {
            Id = al.Id,
            Title = al.Title,
            ArtistName = a.Name,
            ArtistId = a.Id,
            CoverImagePath = al.CoverImagePath,
            ReleaseDate = al.ReleaseDate,
            Genre = al.Genre,
            Songs = []
        }).ToList(),
        Songs = a.Songs.Select(s => new SongListItemViewModel
        {
            Id = s.Id,
            Title = s.Title,
            ArtistName = a.Name,
            AlbumTitle = s.Album?.Title,
            CoverImagePath = s.CoverImagePath ?? s.Album?.CoverImagePath,
            FilePath = s.FilePath,
            DurationSeconds = s.DurationSeconds,
            Genre = s.Genre,
            PlayCount = s.PlayCount
        }).ToList()
    };
}
