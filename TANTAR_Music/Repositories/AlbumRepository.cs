using Microsoft.EntityFrameworkCore;
using TANTAR_Music.Data;
using TANTAR_Music.Models.Domain;

namespace TANTAR_Music.Repositories;

public class AlbumRepository(ApplicationDbContext db) : IAlbumRepository
{
    public async Task<IEnumerable<Album>> GetAllAsync() =>
        await db.Albums.Include(a => a.Artist).Include(a => a.Songs)
            .OrderBy(a => a.Title).ToListAsync();

    public async Task<Album?> GetByIdAsync(int id) =>
        await db.Albums.Include(a => a.Artist).Include(a => a.Songs).ThenInclude(s => s.Artist)
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<IEnumerable<Album>> GetByArtistAsync(int artistId) =>
        await db.Albums.Include(a => a.Artist).Include(a => a.Songs)
            .Where(a => a.ArtistId == artistId).OrderByDescending(a => a.ReleaseDate).ToListAsync();

    public async Task<IEnumerable<Album>> GetRecentAsync(int count) =>
        await db.Albums.Include(a => a.Artist)
            .OrderByDescending(a => a.CreatedAt).Take(count).ToListAsync();

    public async Task<Album> CreateAsync(Album album)
    {
        db.Albums.Add(album);
        await db.SaveChangesAsync();
        return album;
    }

    public async Task<Album> UpdateAsync(Album album)
    {
        db.Albums.Update(album);
        await db.SaveChangesAsync();
        return album;
    }

    public async Task DeleteAsync(int id)
    {
        var album = await db.Albums.FindAsync(id);
        if (album != null)
        {
            db.Albums.Remove(album);
            await db.SaveChangesAsync();
        }
    }
}
