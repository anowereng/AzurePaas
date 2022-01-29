namespace AzurePaas.API.Services
{
    public interface IFileService
    {
        Task<bool> ImageUpload(IFormFile input);
        FileStream GetLocalImage(string filename);
        void DeleteRawFile(string filename);
        Task Upload(FileStream stream, string fileName);
        Task<byte[]> Get(string imageName);
        Task<bool> Delete(string imageName);

    }
}