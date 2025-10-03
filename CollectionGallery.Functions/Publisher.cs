using Google.Protobuf;
using System.Threading.Tasks;
using System;

namespace CollectionGallery.Functions.Services;

[Obsolete("This class is no longer used", error: true)]
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
        Google.Cloud.PubSub.V1.TopicName requestTopicName = Google.Cloud.PubSub.V1.TopicName.FromProjectTopic("files-management-471014", _topicName);

        // Create a PublisherClient to publish messages to the request topic.
        Google.Cloud.PubSub.V1.PublisherClient requestPublisher = await Google.Cloud.PubSub.V1.PublisherClient.CreateAsync(requestTopicName);

        // Generate a unique correlation ID for the request.
        // string correlationId = Guid.NewGuid().ToString();

        // Create the request message, including the correlation ID.
        // string requestMessage = $"{correlationId}:{messageText}";
        // string requestMessage = "Hello, this is from Storage project publisher 👋🏽";

        // Create a PubsubMessage object with the request message data.
        Google.Cloud.PubSub.V1.PubsubMessage message = new Google.Cloud.PubSub.V1.PubsubMessage
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