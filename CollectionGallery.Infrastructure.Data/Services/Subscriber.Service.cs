using System.Text.Json;
using CollectionGallery.InfraStructure.Data.Configurations;
using CollectionGallery.InfraStructure.Data.Enums;
using CollectionGallery.InfraStructure.Data.File.Models;
using CollectionGallery.InfraStructure.Data.Item.Models;
using Google.Apis.Storage.v1;
using Google.Cloud.PubSub.V1;

namespace CollectionGallery.InfraStructure.Data.Services;

public class SubscriberService : BackgroundService
{
    private readonly ILogger<SubscriberService> _logger;
    private readonly DataSecrets _secrets;
    private readonly IServiceScopeFactory _scopeFactory;
    private FileService _fileService;
    private SubscriberClient _subscriberClient { get; set; }

    public SubscriberService(ILogger<SubscriberService> logger, DataSecrets secrets, IServiceScopeFactory scopeFactory)
    {
        _secrets = secrets;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task SubscribeAsync(CancellationToken _)
    {
        await _subscriberClient.StartAsync(async (PubsubMessage message, CancellationToken _) =>
        {
            try
            {
                using IServiceScope scope = _scopeFactory.CreateScope();
                _fileService = scope.ServiceProvider.GetRequiredService<FileService>();
                
                string text = System.Text.Encoding.UTF8.GetString(message.Data.ToArray());
            
                StrorageUploadObjectDto? result = JsonSerializer.Deserialize<StrorageUploadObjectDto>(text);

                if (result is null)
                {
                    return SubscriberClient.Reply.Ack;
                }

                InsertCollectionFileDto payload = new InsertCollectionFileDto
                {
                    ItemId = Convert.ToInt32(result.MetaData.ItemId),
                    Bucket = result.Bucket,
                    Extension = result.ContentType,
                    MimeType = result.ContentType,
                    Name = result.Name,
                    Size = Convert.ToInt64(result.Size)
                };
                
                await _fileService.InsertOneAsync(payload);
                return SubscriberClient.Reply.Ack;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return SubscriberClient.Reply.Ack;
            }
        });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(_secrets.GoogleProjectId, _secrets.PubSub.StorageUploadSubscription);
        _subscriberClient = await SubscriberClient.CreateAsync(subscriptionName);
        
        _logger.LogInformation("Subscriber background service starting...");
        await SubscribeAsync(stoppingToken);
    }
}