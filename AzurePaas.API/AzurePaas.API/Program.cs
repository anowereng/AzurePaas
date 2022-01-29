using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using AzurePaas.API.Controllers;
using AzurePaas.API.Services;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

 static async Task<CosmosDbService> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection)
{
    var databaseName = configurationSection["DatabaseName"];
    var containerName = configurationSection["ContainerName"];
    var account = configurationSection["AccountUrl"];
    var key = configurationSection["AuthKey"];
    var client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
    var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
    await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");
    var cosmosDbService = new CosmosDbService(client, databaseName, containerName);
    return cosmosDbService;
}

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// azure blob 
builder.Services.AddScoped(_ => {
    return new BlobServiceClient(builder.Configuration["ConnectionStrings:AzureBlobStorage"]);
});
// azure cosmos db
builder.Services.AddSingleton<ICosmosDbService>(InitializeCosmosClientInstanceAsync(builder.Configuration.GetSection("Cosmos")).GetAwaiter().GetResult());

// background service 
builder.Services.AddHostedService<MyQueueBackgroundService>();
builder.Services.AddScoped<IFileService, FileService>();

// azure extension service 
builder.Services.AddAzureClients(client =>
{
    client.AddClient<QueueClient, QueueClientOptions>((_, _, _) =>
    {
        var connectionQueue = builder.Configuration["Queue:Connection"];
        var queueName = builder.Configuration["Queue:QueueName"];
        return new QueueClient(connectionQueue, queueName);
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();
