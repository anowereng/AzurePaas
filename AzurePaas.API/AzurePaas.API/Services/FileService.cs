using Azure.Storage.Blobs;
using AzurePaas.API.Models;

namespace AzurePaas.API.Services
{
    public class FileService : IFileService
    {
        private IWebHostEnvironment _env;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ICosmosDbService _cosmosDbService;

        public FileService(IWebHostEnvironment env, BlobServiceClient blobServiceClient, ICosmosDbService cosmosDbService)
        {
            this._env = env;
            this._blobServiceClient = blobServiceClient;
            this._cosmosDbService = cosmosDbService;
        }


        public async Task<bool> ImageUpload(IFormFile input)
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
            //await AzureUpload(input);

            var file = new UploadFile
            {
                Id = Guid.NewGuid().ToString(),
                FileName = uniqueFileName,
                FileType = input.FileName.Split(".")[1]
            };
           await _cosmosDbService.AddAsync(file);
            return true;
        }
        public async Task AzureUpload(IFormFile file)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient("uploadfiles");
            var blobClient = blobContainer.GetBlobClient(file.FileName);
            await blobClient.UploadAsync(file.OpenReadStream());
        }

        private string GetUniqueFileName(string fileName)
        {
            return Guid.NewGuid().ToString()
                   + Path.GetExtension(fileName);
        }
        public async Task<byte[]> Get(string imageName)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient("uploadfiles");

            var blobClient = blobContainer.GetBlobClient(imageName);
            var downloadContent = await blobClient.DownloadAsync();
            using (MemoryStream ms = new MemoryStream())
            {
                await downloadContent.Value.Content.CopyToAsync(ms);
                return ms.ToArray();
            }
        }

        public async Task<bool> Delete(string imageName)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient("uploadfiles");

            var blobClient = blobContainer.GetBlobClient(imageName);
            var response = await blobClient.DeleteIfExistsAsync();
            return response;
        }

        public FileStream GetLocalImage(string filename)
        {
            throw new NotImplementedException();
        }

        public void DeleteRawFile(string filename)
        {
            throw new NotImplementedException();
        }

        public Task Upload(FileStream stream, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
