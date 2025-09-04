using Google.Protobuf;
using Google.Cloud.PubSub.V1;
using CollectionGallery.Shared;

namespace CollectionGallery.Infrastructure.Storage.Services;

public class PublisherService
{
    private readonly string _topicName = "files-management";
    private readonly string _projectId;

    public PublisherService()
    {
        _projectId = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT_ID")!;
    }

    /// <summary>
    /// Publishes a message to a Google Cloud Pub/Sub topic.
    /// </summary>
    /// <param name="requestMessage">The string content of the message payload.</param>
    /// <param name="traceId">A unique identifier for tracing the message across services. This will be added as an attribute.</param>
    /// <param name="eventType">The type of the event being published (e.g., "file_uploaded"). This will be added as an attribute.</param>
    /// <returns>A task that represents the asynchronous publish operation. The task's result is the unique ID of the published message.</returns>
    public async Task PublishMessageAsync(string requestMessage, string traceId, string eventType)
    {
        // Create a TopicName object for the request topic.
        TopicName requestTopicName = TopicName.FromProjectTopic(_projectId, _topicName);

        // Create a PublisherClient to publish messages to the request topic.
        PublisherClient requestPublisher = await PublisherClient.CreateAsync(requestTopicName);

        // Create a PubsubMessage object with the request message data.
        PubsubMessage message = new PubsubMessage
        {
            Data = ByteString.CopyFromUtf8(requestMessage),
            Attributes = {
                {"event", eventType},
                {"trace-id", traceId}
            }
        };

        // Publish the request message.
        await requestPublisher.PublishAsync(message);
        Logger.LogInformation($"Message with Trace ID: {traceId} was successfully published");
    }
}