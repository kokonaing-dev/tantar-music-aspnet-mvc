using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TANTAR_Music.Models.ViewModels;
using TANTAR_Music.Services;

namespace TANTAR_Music.Controllers;

public class AlbumController(IAlbumService albumService) : Controller
{
    public async Task<IActionResult> Index() => View(await albumService.GetAllAsync());

    public async Task<IActionResult> Details(int id)
    {
        var album = await albumService.GetByIdAsync(id);
        if (album == null) return NotFound();
        return View(album);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create() => View(await albumService.GetCreateFormAsync());

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(AlbumViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var form = await albumService.GetCreateFormAsync();
            model.Artists = form.Artists;
            return View(model);
        }

        await albumService.CreateAsync(model);
        TempData["Success"] = "Album created!";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await albumService.GetEditFormAsync(id);
        if (model == null) return NotFound();
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(AlbumViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var form = await albumService.GetCreateFormAsync();
            model.Artists = form.Artists;
            return View(model);
        }

        await albumService.UpdateAsync(model);
        TempData["Success"] = "Album updated!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await albumService.DeleteAsync(id);
        TempData["Success"] = "Album deleted.";
        return RedirectToAction(nameof(Index));
    }
}
