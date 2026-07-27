using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TANTAR_Music.Models.ViewModels;
using TANTAR_Music.Services;

namespace TANTAR_Music.Controllers;

public class ArtistController(IArtistService artistService) : Controller
{
    public async Task<IActionResult> Index() => View(await artistService.GetAllAsync());

    public async Task<IActionResult> Details(int id)
    {
        var artist = await artistService.GetByIdAsync(id);
        if (artist == null) return NotFound();
        return View(artist);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create() => View(await artistService.GetCreateFormAsync());

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(ArtistViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        await artistService.CreateAsync(model);
        TempData["Success"] = "Artist created!";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await artistService.GetEditFormAsync(id);
        if (model == null) return NotFound();
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(ArtistViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        await artistService.UpdateAsync(model);
        TempData["Success"] = "Artist updated!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await artistService.DeleteAsync(id);
        TempData["Success"] = "Artist deleted.";
        return RedirectToAction(nameof(Index));
    }
}
