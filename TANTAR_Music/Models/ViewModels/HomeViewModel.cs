namespace TANTAR_Music.Models.ViewModels;

public class HomeViewModel
{
    public List<SongListItemViewModel> RecentSongs { get; set; } = [];
    public List<AlbumDetailViewModel> RecentAlbums { get; set; } = [];
    public List<ArtistDetailViewModel> FeaturedArtists { get; set; } = [];
    public List<SongListItemViewModel> TopSongs { get; set; } = [];
}
