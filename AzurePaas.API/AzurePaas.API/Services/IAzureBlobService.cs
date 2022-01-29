namespace AzurePaas.API.Services
{
    public interface IAzureBlobService
    {
        Task Upload(FileStream stream, string fileName);
        Task<byte[]> Get(string imageName);
        Task<bool> Delete(string imageName);
    }
}