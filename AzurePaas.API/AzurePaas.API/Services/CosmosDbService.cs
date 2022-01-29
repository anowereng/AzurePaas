using AzurePaas.API.Models;
using Microsoft.Azure.Cosmos;

namespace AzurePaas.API.Services
{
    public class CosmosDbService : ICosmosDbService
    {
        private Container _container;

        public CosmosDbService(
            CosmosClient cosmosDbClient,
            string databaseName,
            string containerName)
        {
            _container = cosmosDbClient.GetContainer(databaseName, containerName);
        }

        public async Task AddAsync(UploadFile item)
        {
            await _container.CreateItemAsync(item, new PartitionKey(item.Id));
        }

        public async Task DeleteAsync(string id)
        {
            await _container.DeleteItemAsync<UploadFile>(id, new PartitionKey(id));
        }

        public async Task<UploadFile> GetAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<UploadFile>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException) //For handling item not found and other exceptions
            {
                return null;
            }
        }

        public async Task<IEnumerable<UploadFile>> GetMultipleAsync(string queryString)
        {
            var query = _container.GetItemQueryIterator<UploadFile>(new QueryDefinition(queryString));

            var results = new List<UploadFile>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateAsync(string id, UploadFile item)
        {
            await _container.UpsertItemAsync(item, new PartitionKey(id));
        }
    }
}
