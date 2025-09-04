using System.Text.Json;
using CollectionGallery.Domain.Models.Controllers;
using Google.Cloud.PubSub.V1;
using CollectionGallery.Shared;
using CollectionGallery.Domain.Models.Enums;

namespace CollectionGallery.InfraStructure.Data.Services;

public class SubscriberService
{
    private readonly FileService _fileService;
    private readonly string _subscriberId;
    private readonly string _projectId;

    public SubscriberService(FileService service)
    {
        _fileService = service;
        _projectId = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT_ID") ?? "";
        _subscriberId = "files-management-sub";
    }

    public async Task SubscribeAsync()
    {
        SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(_projectId, _subscriberId);
        SubscriberClient subscriber = await SubscriberClient.CreateAsync(subscriptionName);
        
        await subscriber.StartAsync(async (PubsubMessage message, CancellationToken _) =>
        {
            string text = System.Text.Encoding.UTF8.GetString(message.Data.ToArray());
            string traceId = message.Attributes["trace-id"];
            if (message.Attributes["event"] == "FileUpload")
            {
                FileUploadResultObject? resultObject = JsonSerializer.Deserialize<FileUploadResultObject>(text);
                if (resultObject is not null)
                {
                    Logger.LogInformation($"Received message at {subscriber.SubscriptionName} subscriber with Trace ID: {traceId}");
                    MethodStatus status = await _fileService.InsertFileAsync(resultObject);
                    return SubscriberClient.Reply.Ack;
                }
                else
                {
                    Logger.LogWarning($"No Data {text} was received to the Subscriber {subscriber} with Trace ID {traceId}");
                    return SubscriberClient.Reply.Nack;
                }
            }
            
            return SubscriberClient.Reply.Ack;
        });

        Console.WriteLine($"Listening for messages on {subscriptionName}");
        Console.ReadKey(); // Keep the subscriber running
    }
}