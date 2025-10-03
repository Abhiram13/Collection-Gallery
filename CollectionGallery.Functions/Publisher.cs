using Google.Protobuf;
using Google.Cloud.PubSub.V1;
using System.Threading.Tasks;
using System;

namespace CollectionGallery.Functions.Services;

// [Obsolete("This class is no longer used", error: true)]
public class PublisherService
{
    private readonly string _topicName = "collection-gallery-topic";
    private readonly string _projectId;

    public PublisherService()
    {
        _projectId = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT_ID");
    }

    public async Task PublishMessageAsync(string requestMessage)
    {
        // Create a TopicName object for the request topic.
        // TopicName requestTopicName = TopicName.FromProjectTopic("files-management-471014", _topicName);
        TopicName requestTopicName = TopicName.Parse($"projects/files-management-471014/topics/{_topicName}");

        // Create a PublisherClient to publish messages to the request topic.
        PublisherClient requestPublisher = await PublisherClient.CreateAsync(requestTopicName);

        // Generate a unique correlation ID for the request.
        // string correlationId = Guid.NewGuid().ToString();

        // Create the request message, including the correlation ID.
        // string requestMessage = $"{correlationId}:{messageText}";
        // string requestMessage = "Hello, this is from Storage project publisher 👋🏽";

        // Create a PubsubMessage object with the request message data.
        PubsubMessage message = new PubsubMessage
        {
            Data = ByteString.CopyFromUtf8(requestMessage)
        };

        // Create a TaskCompletionSource to wait for the response.
        // TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

        // Add the TaskCompletionSource to the PendingRequests dictionary, using the correlation ID as the key.
        // PendingRequests.TryAdd(correlationId, tcs);

        // Publish the request message.
        await requestPublisher.PublishAsync(message);
        Console.WriteLine($"Published request message: {requestMessage}");

        // Start listening for responses on the response subscription.
        // await SubscribeToResponse(projectId, responseSubscriptionId);

        // Wait for the response and get the response data from the TaskCompletionSource.
        // string response = await tcs.Task;
        // Console.WriteLine($"received response: {response}");
    }
}