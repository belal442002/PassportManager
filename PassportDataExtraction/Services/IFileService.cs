namespace Document_Intelligence_Task.Services
{
    public interface IFileService
    {
        Task<string?> SaveFileAsync(IFormFile file);
        bool DeleteFile(string filePath);
    }
}
