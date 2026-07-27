using Microsoft.EntityFrameworkCore;
using TANTAR_Music.Data;
using TANTAR_Music.Models.Domain;

namespace TANTAR_Music.Repositories;

public class SongRepository(ApplicationDbContext db) : ISongRepository
{
    public async Task<IEnumerable<Song>> GetAllAsync() =>
        await db.Songs.Include(s => s.Artist).Include(s => s.Album).OrderBy(s => s.Title).ToListAsync();

    public async Task<Song?> GetByIdAsync(int id) =>
        await db.Songs.Include(s => s.Artist).Include(s => s.Album).FirstOrDefaultAsync(s => s.Id == id);

    public async Task<IEnumerable<Song>> GetByArtistAsync(int artistId) =>
        await db.Songs.Include(s => s.Artist).Include(s => s.Album)
            .Where(s => s.ArtistId == artistId).OrderBy(s => s.Title).ToListAsync();

    public async Task<IEnumerable<Song>> GetByAlbumAsync(int albumId) =>
        await db.Songs.Include(s => s.Artist)
            .Where(s => s.AlbumId == albumId).OrderBy(s => s.Title).ToListAsync();

    public async Task<IEnumerable<Song>> GetTopAsync(int count) =>
        await db.Songs.Include(s => s.Artist).Include(s => s.Album)
            .OrderByDescending(s => s.PlayCount).Take(count).ToListAsync();

    public async Task<IEnumerable<Song>> GetRecentAsync(int count) =>
        await db.Songs.Include(s => s.Artist).Include(s => s.Album)
            .OrderByDescending(s => s.CreatedAt).Take(count).ToListAsync();

    public async Task<IEnumerable<Song>> SearchAsync(string query) =>
        await db.Songs.Include(s => s.Artist).Include(s => s.Album)
            .Where(s => s.Title.Contains(query) || s.Artist.Name.Contains(query) || (s.Genre != null && s.Genre.Contains(query)))
            .OrderBy(s => s.Title).ToListAsync();

    public async Task<Song> CreateAsync(Song song)
    {
        db.Songs.Add(song);
        await db.SaveChangesAsync();
        return song;
    }

    public async Task<Song> UpdateAsync(Song song)
    {
        db.Songs.Update(song);
        await db.SaveChangesAsync();
        return song;
    }

    public async Task DeleteAsync(int id)
    {
        var song = await db.Songs.FindAsync(id);
        if (song != null)
        {
            db.Songs.Remove(song);
            await db.SaveChangesAsync();
        }
    }

    public async Task IncrementPlayCountAsync(int id)
    {
        await db.Songs.Where(s => s.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.PlayCount, x => x.PlayCount + 1));
    }
}
