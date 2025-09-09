using CollectionGallery.InfraStructure.Data.Services;

public class SubscriberBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SubscriberBackgroundService> _logger;

    public SubscriberBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<SubscriberBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Subscriber background service starting...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var subscriberService = scope.ServiceProvider.GetRequiredService<SubscriberService>();
                await subscriberService.SubscribeAsync(); // Keep listening
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in subscriber loop");
                await Task.Delay(5000, stoppingToken); // backoff before retry
            }
        }

        _logger.LogInformation("Subscriber background service stopping.");
    }
}
