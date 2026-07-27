using Microsoft.EntityFrameworkCore;
using TANTAR_Music.Data;
using TANTAR_Music.Models.Domain;

namespace TANTAR_Music.Repositories;

public class PlaylistRepository(ApplicationDbContext db) : IPlaylistRepository
{
    public async Task<IEnumerable<Playlist>> GetByUserAsync(string userId) =>
        await db.Playlists
            .Include(p => p.PlaylistSongs)
                .ThenInclude(ps => ps.Song).ThenInclude(s => s.Artist)
            .Include(p => p.PlaylistSongs)
                .ThenInclude(ps => ps.Song).ThenInclude(s => s.Album)
            .Where(p => p.UserId == userId).OrderBy(p => p.Name).ToListAsync();

    public async Task<IEnumerable<Playlist>> GetPublicAsync() =>
        await db.Playlists
            .Include(p => p.User)
            .Include(p => p.PlaylistSongs)
                .ThenInclude(ps => ps.Song).ThenInclude(s => s.Artist)
            .Include(p => p.PlaylistSongs)
                .ThenInclude(ps => ps.Song).ThenInclude(s => s.Album)
            .Where(p => p.IsPublic).OrderByDescending(p => p.CreatedAt).ToListAsync();

    public async Task<Playlist?> GetByIdAsync(int id) =>
        await db.Playlists
            .Include(p => p.User)
            .Include(p => p.PlaylistSongs.OrderBy(ps => ps.Order))
                .ThenInclude(ps => ps.Song).ThenInclude(s => s.Artist)
            .Include(p => p.PlaylistSongs)
                .ThenInclude(ps => ps.Song).ThenInclude(s => s.Album)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Playlist> CreateAsync(Playlist playlist)
    {
        db.Playlists.Add(playlist);
        await db.SaveChangesAsync();
        return playlist;
    }

    public async Task<Playlist> UpdateAsync(Playlist playlist)
    {
        db.Playlists.Update(playlist);
        await db.SaveChangesAsync();
        return playlist;
    }

    public async Task DeleteAsync(int id)
    {
        var playlist = await db.Playlists.FindAsync(id);
        if (playlist != null)
        {
            db.Playlists.Remove(playlist);
            await db.SaveChangesAsync();
        }
    }

    public async Task AddSongAsync(int playlistId, int songId)
    {
        var exists = await db.PlaylistSongs.AnyAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);
        if (exists) return;

        var maxOrder = await db.PlaylistSongs.Where(ps => ps.PlaylistId == playlistId)
            .Select(ps => (int?)ps.Order).MaxAsync() ?? 0;

        db.PlaylistSongs.Add(new PlaylistSong
        {
            PlaylistId = playlistId,
            SongId = songId,
            Order = maxOrder + 1,
            AddedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
    }

    public async Task RemoveSongAsync(int playlistId, int songId)
    {
        var entry = await db.PlaylistSongs.FindAsync(playlistId, songId);
        if (entry != null)
        {
            db.PlaylistSongs.Remove(entry);
            await db.SaveChangesAsync();
        }
    }
}
