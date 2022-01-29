using Azure.Storage.Queues;
using AzurePaas.API.Models;
using System.Text.Json;

namespace AzurePaas.API.Services
{
    public class MyQueueBackgroundService : BackgroundService
    {
        private readonly ILogger<MyQueueBackgroundService> _logger;
        private readonly QueueClient _queueClient;
        public MyQueueBackgroundService(ILogger<MyQueueBackgroundService> logger, QueueClient quueueClient)
        {
            _logger = logger;
            _queueClient = quueueClient;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var message = await _queueClient.ReceiveMessageAsync();
                _logger.LogInformation("Message reading..");

                if (message.Value != null)
                {
                    var messagedata = JsonSerializer.Deserialize<UploadFile>(message.Value.MessageText);
                    _logger.LogInformation("New Message {0}", message.Value.MessageText);
                    await _queueClient.DeleteMessageAsync(message.Value.MessageId, message.Value.PopReceipt);
                }
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
    
        }
    }
}
