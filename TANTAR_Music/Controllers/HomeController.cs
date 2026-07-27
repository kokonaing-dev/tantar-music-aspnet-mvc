using Microsoft.AspNetCore.Mvc;
using TANTAR_Music.Models.ViewModels;
using TANTAR_Music.Services;

namespace TANTAR_Music.Controllers;

public class HomeController(ISongService songService, IAlbumService albumService, IArtistService artistService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var model = new HomeViewModel
        {
            RecentSongs = (await songService.GetRecentAsync(8)).ToList(),
            TopSongs = (await songService.GetTopAsync(5)).ToList(),
            RecentAlbums = (await albumService.GetRecentAsync(6)).ToList(),
            FeaturedArtists = (await artistService.GetFeaturedAsync(6)).ToList()
        };
        return View(model);
    }

    public async Task<IActionResult> Search(string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return View(new List<SongListItemViewModel>());

        var results = await songService.SearchAsync(q);
        ViewBag.Query = q;
        return View(results);
    }
}
