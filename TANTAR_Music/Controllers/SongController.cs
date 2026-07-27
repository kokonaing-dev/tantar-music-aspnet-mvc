using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TANTAR_Music.Services;
using TANTAR_Music.Models.ViewModels;

namespace TANTAR_Music.Controllers;

public class SongController(ISongService songService) : Controller
{
    public async Task<IActionResult> Index() => View(await songService.GetAllAsync());

    public async Task<IActionResult> Details(int id)
    {
        var song = await songService.GetByIdAsync(id);
        if (song == null) return NotFound();
        return View(song);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create() => View(await songService.GetCreateFormAsync());

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(SongViewModel model)
    {
        if (model.AudioFile == null)
            ModelState.AddModelError(nameof(model.AudioFile), "Audio file is required.");

        if (!ModelState.IsValid)
        {
            var form = await songService.GetCreateFormAsync();
            model.Artists = form.Artists;
            model.Albums = form.Albums;
            return View(model);
        }

        try
        {
            await songService.CreateAsync(model);
            TempData["Success"] = "Song uploaded successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var form = await songService.GetCreateFormAsync();
            model.Artists = form.Artists;
            model.Albums = form.Albums;
            return View(model);
        }
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await songService.GetEditFormAsync(id);
        if (model == null) return NotFound();
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(SongViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var form = await songService.GetCreateFormAsync();
            model.Artists = form.Artists;
            model.Albums = form.Albums;
            return View(model);
        }

        await songService.UpdateAsync(model);
        TempData["Success"] = "Song updated successfully!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await songService.DeleteAsync(id);
        TempData["Success"] = "Song deleted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Play(int id)
    {
        await songService.IncrementPlayCountAsync(id);
        return Ok();
    }
}
