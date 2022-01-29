using Azure.Storage.Blobs;

namespace AzurePaas.API.Services
{
    public class AzureBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        public AzureBlobService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }
        public async Task Upload(FileStream stream, string fileName)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient("uploadfiles");
            var blobClient = blobContainer.GetBlobClient(fileName);
            await blobClient.UploadAsync(stream);
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
    }
}
