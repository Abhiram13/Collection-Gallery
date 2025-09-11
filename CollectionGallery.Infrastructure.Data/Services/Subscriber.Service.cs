using System.Text.Json;
using CollectionGallery.Domain.Models.Controllers;
using Google.Cloud.PubSub.V1;
using CollectionGallery.Domain.Models.Enums;
using Google.Apis.Storage.v1;

namespace CollectionGallery.InfraStructure.Data.Services;

public class SubscriberService
{
    private readonly ItemService _itemService;
    private readonly string _subscriberId;
    private readonly string _projectId;
    private readonly ILogger<StorageService> _logger;

    public SubscriberService(ItemService service, ILogger<StorageService> logger)
    {
        _itemService = service;
        _projectId = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT_ID") ?? "";
        _subscriberId = "files-management-sub";
        _logger = logger;
    }

    public async Task SubscribeAsync(CancellationToken cancellationToken)
    {
        SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(_projectId, _subscriberId);
        SubscriberClient subscriber = await SubscriberClient.CreateAsync(subscriptionName);
        
        Task subscriberTask = subscriber.StartAsync(async (PubsubMessage message, CancellationToken _) =>
        {
            string text = System.Text.Encoding.UTF8.GetString(message.Data.ToArray());
            string traceId = message.Attributes["trace-id"];
            if (message.Attributes["event"] == "FileUpload")
            {
                FileUploadResultObject? resultObject = JsonSerializer.Deserialize<FileUploadResultObject>(text);
                if (resultObject is not null)
                {
                    _logger.LogInformation($"Received message at {subscriber.SubscriptionName} subscriber with Trace ID: {traceId}");
                    MethodStatus status = await _itemService.InsertItemAsync(resultObject);
                    return SubscriberClient.Reply.Ack;
                }
                else
                {
                    _logger.LogWarning($"No Data {text} was received to the Subscriber {subscriber} with Trace ID {traceId}");
                    return SubscriberClient.Reply.Nack;
                }
            }
            
            return SubscriberClient.Reply.Ack;
        });

        try
        {
            await Task.Delay(Timeout.Infinite, cancellationToken); // Keeps it alive
        }
        catch (OperationCanceledException)
        {
            await subscriber.StopAsync(CancellationToken.None); // Stop listener
        }

        await subscriberTask;

        _logger.LogInformation($"Listening for messages on {subscriptionName}");
    }
}