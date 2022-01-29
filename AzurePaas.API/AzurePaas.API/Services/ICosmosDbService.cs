using AzurePaas.API.Models;

namespace AzurePaas.API.Services
{
    public interface ICosmosDbService
    {
        Task<IEnumerable<UploadFile>> GetMultipleAsync(string query);
        Task<UploadFile> GetAsync(string id);
        Task AddAsync(UploadFile item);
        Task UpdateAsync(string id, UploadFile item);
        Task DeleteAsync(string id);
    }
}
