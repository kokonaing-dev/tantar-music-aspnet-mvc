namespace TANTAR_Music.Services;

public interface IFileService
{
    Task<string> SaveAudioAsync(IFormFile file);
    Task<string> SaveCoverAsync(IFormFile file, string subfolder = "covers");
    Task<int> GetAudioDurationAsync(string filePath);
    void DeleteFile(string relativePath);
}
