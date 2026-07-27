using Microsoft.EntityFrameworkCore;
using TANTAR_Music.Data;
using TANTAR_Music.Models.Domain;

namespace TANTAR_Music.Repositories;

public class ArtistRepository(ApplicationDbContext db) : IArtistRepository
{
    public async Task<IEnumerable<Artist>> GetAllAsync() =>
        await db.Artists.Include(a => a.Albums).Include(a => a.Songs)
            .OrderBy(a => a.Name).ToListAsync();

    public async Task<Artist?> GetByIdAsync(int id) =>
        await db.Artists.Include(a => a.Albums).ThenInclude(al => al.Songs)
            .Include(a => a.Songs).ThenInclude(s => s.Album)
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<IEnumerable<Artist>> GetFeaturedAsync(int count) =>
        await db.Artists.Include(a => a.Songs)
            .OrderByDescending(a => a.Songs.Sum(s => s.PlayCount)).Take(count).ToListAsync();

    public async Task<Artist> CreateAsync(Artist artist)
    {
        db.Artists.Add(artist);
        await db.SaveChangesAsync();
        return artist;
    }

    public async Task<Artist> UpdateAsync(Artist artist)
    {
        db.Artists.Update(artist);
        await db.SaveChangesAsync();
        return artist;
    }

    public async Task DeleteAsync(int id)
    {
        var artist = await db.Artists.FindAsync(id);
        if (artist != null)
        {
            db.Artists.Remove(artist);
            await db.SaveChangesAsync();
        }
    }
}
