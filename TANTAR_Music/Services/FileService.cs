namespace TANTAR_Music.Services;

public class FileService(IWebHostEnvironment env) : IFileService
{
    private static readonly HashSet<string> AllowedAudio = [".mp3", ".wav", ".ogg", ".flac", ".m4a"];
    private static readonly HashSet<string> AllowedImage = [".jpg", ".jpeg", ".png", ".gif", ".webp"];

    public async Task<string> SaveAudioAsync(IFormFile file)
    {
        ValidateExtension(file.FileName, AllowedAudio, "audio");
        return await SaveFile(file, "audio");
    }

    public async Task<string> SaveCoverAsync(IFormFile file, string subfolder = "covers")
    {
        ValidateExtension(file.FileName, AllowedImage, "image");
        return await SaveFile(file, subfolder);
    }

    public Task<int> GetAudioDurationAsync(string relativePath)
    {
        // Returns 0 when duration detection is unavailable; can be upgraded with a media library later
        return Task.FromResult(0);
    }

    public void DeleteFile(string relativePath)
    {
        var fullPath = Path.Combine(env.WebRootPath, relativePath.TrimStart('/'));
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }

    private async Task<string> SaveFile(IFormFile file, string subfolder)
    {
        var dir = Path.Combine(env.WebRootPath, "uploads", subfolder);
        Directory.CreateDirectory(dir);

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid()}{ext}";
        var fullPath = Path.Combine(dir, fileName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/uploads/{subfolder}/{fileName}";
    }

    private static void ValidateExtension(string fileName, HashSet<string> allowed, string type)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        if (!allowed.Contains(ext))
            throw new InvalidOperationException($"Invalid {type} file type: {ext}");
    }
}
