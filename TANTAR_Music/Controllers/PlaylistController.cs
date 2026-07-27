using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TANTAR_Music.Models.Domain;
using TANTAR_Music.Models.ViewModels;
using TANTAR_Music.Services;

namespace TANTAR_Music.Controllers;

[Authorize]
public class PlaylistController(IPlaylistService playlistService, UserManager<ApplicationUser> userManager) : Controller
{
    public async Task<IActionResult> Index()
    {
        var userId = userManager.GetUserId(User)!;
        return View(await playlistService.GetUserPlaylistsAsync(userId));
    }

    [AllowAnonymous]
    public async Task<IActionResult> Public() => View(await playlistService.GetPublicPlaylistsAsync());

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var playlist = await playlistService.GetByIdAsync(id);
        if (playlist == null) return NotFound();

        var userId = userManager.GetUserId(User);
        if (!playlist.IsPublic && playlist.OwnerId != userId)
            return User.Identity?.IsAuthenticated == true ? Forbid() : Challenge();

        ViewBag.IsOwner = playlist.OwnerId == userId;
        return View(playlist);
    }

    public IActionResult Create() => View(new PlaylistViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PlaylistViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var userId = userManager.GetUserId(User)!;
        var id = await playlistService.CreateAsync(model, userId);
        TempData["Success"] = "Playlist created!";
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var playlist = await playlistService.GetByIdAsync(id);
        if (playlist == null) return NotFound();
        if (playlist.OwnerId != userManager.GetUserId(User)) return Forbid();

        return View(new PlaylistViewModel
        {
            Id = playlist.Id,
            Name = playlist.Name,
            Description = playlist.Description,
            IsPublic = playlist.IsPublic,
            ExistingCoverPath = playlist.CoverImagePath
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(PlaylistViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var playlist = await playlistService.GetByIdAsync(model.Id);
        if (playlist == null) return NotFound();
        if (playlist.OwnerId != userManager.GetUserId(User)) return Forbid();

        await playlistService.UpdateAsync(model);
        TempData["Success"] = "Playlist updated!";
        return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var playlist = await playlistService.GetByIdAsync(id);
        if (playlist == null) return NotFound();
        if (playlist.OwnerId != userManager.GetUserId(User)) return Forbid();

        await playlistService.DeleteAsync(id);
        TempData["Success"] = "Playlist deleted.";
        return RedirectToAction(nameof(Index));
    }

    // Returns current user's playlists as JSON for the "Add to playlist" dropdown
    [HttpGet]
    public async Task<IActionResult> GetMyPlaylists()
    {
        var userId = userManager.GetUserId(User)!;
        var playlists = await playlistService.GetUserPlaylistsAsync(userId);
        return Json(playlists.Select(p => new { p.Id, p.Name }));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddSong(int playlistId, int songId)
    {
        var playlist = await playlistService.GetByIdAsync(playlistId);
        if (playlist == null) return NotFound();
        if (playlist.OwnerId != userManager.GetUserId(User)) return Forbid();

        await playlistService.AddSongAsync(playlistId, songId);
        return Ok(new { message = "Song added to playlist." });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveSong(int playlistId, int songId)
    {
        var playlist = await playlistService.GetByIdAsync(playlistId);
        if (playlist == null) return NotFound();
        if (playlist.OwnerId != userManager.GetUserId(User)) return Forbid();

        await playlistService.RemoveSongAsync(playlistId, songId);
        return Ok();
    }
}
