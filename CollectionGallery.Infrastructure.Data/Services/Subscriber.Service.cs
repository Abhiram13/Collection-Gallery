using System.Text.Json;
using CollectionGallery.InfraStructure.Data.Configurations;
using CollectionGallery.InfraStructure.Data.Enums;
using CollectionGallery.InfraStructure.Data.Item.Models;
using Google.Apis.Storage.v1;
using Google.Cloud.PubSub.V1;

namespace CollectionGallery.InfraStructure.Data.Services;

public class SubscriberService
{
    private readonly string _subscriberId;
    private readonly string _projectId;
    private readonly ILogger<StorageService> _logger;
    private readonly DataSecrets _secrets;

    public SubscriberService(ILogger<StorageService> logger, DataSecrets secrets)
    {
        _secrets = secrets;
        _projectId = _secrets.GoogleProjectId;
        _subscriberId = _secrets.PubSub.StorageUploadSubscription;
        _logger = logger;
    }

    public async Task SubscribeAsync(CancellationToken cancellationToken)
    {
        SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(_projectId, _subscriberId);
        SubscriberClient subscriber = await SubscriberClient.CreateAsync(subscriptionName);
        
        cancellationToken.Register(() =>
        {
            subscriber.StopAsync(TimeSpan.FromSeconds(5));
        });
        
        Task subscriberTask = subscriber.StartAsync(async (PubsubMessage message, CancellationToken _) =>
        {
            string text = System.Text.Encoding.UTF8.GetString(message.Data.ToArray());
            _logger.LogInformation(text);
            // string traceId = message.Attributes["trace-id"];
            // if (message.Attributes["event"] == "FileUpload")
            // {
            //     FileUploadResultObject? resultObject = JsonSerializer.Deserialize<FileUploadResultObject>(text);
            //     if (resultObject is not null)
            //     {
            //         _logger.LogInformation($"Received message at {subscriber.SubscriptionName} subscriber with Trace ID: {traceId}");
            //         // MethodStatus status = await _itemService.InsertItemAsync(resultObject);
            //         return SubscriberClient.Reply.Ack;
            //     }
            //     else
            //     {
            //         _logger.LogWarning($"No Data {text} was received to the Subscriber {subscriber} with Trace ID {traceId}");
            //         return SubscriberClient.Reply.Nack;
            //     }
            // }
            
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