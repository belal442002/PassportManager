
namespace Document_Intelligence_Task.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public bool DeleteFile(string filePath)
        {
            try
            {
                var test = _env.WebRootPath;
                var physicalPath = Path.Combine(_env.ContentRootPath, "wwwroot", filePath).Replace("\\", "/");
                if(System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                    return true;
                }
                return false;
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string?> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null; 

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var folderName = "files"; // name of the folder that will store the documents

            var uploadsFolder = Path.Combine(_env.WebRootPath, folderName);

            if(!Directory.Exists(uploadsFolder)) 
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string filePath = Path.Combine(uploadsFolder, uniqueFileName).Replace("\\", "/");

            using(var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return Path.Combine(folderName, uniqueFileName).Replace("\\", "/"); 
        }
    }
}
