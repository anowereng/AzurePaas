namespace AzurePaas.API.Controllers
{
    public class FileService
    {

        public async Task<string> ImageUpload(IFormFile input, IWebHostEnvironment _env)
        {
            var uniqueFileName = GetUniqueFileName(input.FileName);

            var fullRootPathDir = Path.Combine(_env.WebRootPath, "Upload");
            var fullImagePathDir = Path.Combine(fullRootPathDir, "Images");

            var fullFileName = Path.Combine(fullImagePathDir, uniqueFileName);

            if (!Directory.Exists(fullImagePathDir))
            {
                Directory.CreateDirectory(fullImagePathDir);
            }
            await input.CopyToAsync(new FileStream(fullFileName, FileMode.Create));
            return uniqueFileName;
        }

        private string GetUniqueFileName(string fileName)
        {
            return Guid.NewGuid().ToString()
                   + Path.GetExtension(fileName);
        }

    }
}
