using System.Net;
using Microsoft.EntityFrameworkCore;
using Google.Cloud.Diagnostics.AspNetCore3;
using Google.Cloud.Diagnostics.Common;
using CollectionGallery.InfraStructure.Data;
using CollectionGallery.InfraStructure.Data.Services;
using Abhiram.Extensions.DotEnv;
using Abhiram.Abstractions.Logging;
using Abhiram.Secrets.Providers;
using Abhiram.Secrets.Providers.Interface;
using Microsoft.EntityFrameworkCore.Storage;
using CollectionGallery.Domain.Models.Entities;
using Polly;
using Polly.Retry;
using Polly.Extensions.Http;
using CollectionGallery.Infrastructure.Data;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
DotEnvironmentVariables.Load();

builder.AddConsoleGoogleSeriLog();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddRouting();
builder.Services.AddHostedService<SubscriberBackgroundService>();
builder.Services.AddScoped<ISecretManager, SecretManagerService>();
builder.Services.AddScoped<SubscriberService>();
builder.Services.AddScoped<ModelService>();
builder.Services.AddScoped<ItemService>();
builder.Services.AddScoped<CollectionService>();
builder.Services.AddScoped<PlatformService>();
builder.Services.AddScoped<TagService>();
builder.Services.AddHttpClient<StorageHttpClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:3000");
}).AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 2,     // after 2 consecutive failures
        durationOfBreak: TimeSpan.FromSeconds(10),   // open circuit for 10 seconds
        onBreak: (outcome, ts) => Console.WriteLine($"Circuit opened for {ts.TotalSeconds}s due to {outcome.Result?.StatusCode}"),
        onReset: () => Console.WriteLine("Circuit reset"),
        onHalfOpen: () => Console.WriteLine("Circuit half-open")
    ));
    // .WaitAndRetryAsync(
    //     retryCount: 3,
    //     sleepDurationProvider: retry => TimeSpan.FromSeconds(Math.Pow(2, retry)), // exponential backoff
    //     onRetry: (outcome, timespan, retryAttempt, context) =>
    //     {
    //         Console.WriteLine($"Retry {retryAttempt} after {timespan.TotalSeconds}s due to {outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()}");
    //     }));
builder.Services.AddDbContext<CollectionGalleryContext>(async (provider, options) =>
{
    ISecretManager secretManager = provider.GetRequiredService<ISecretManager>();
    string? postgresHost = await secretManager.GetSecretAsync("POSTGRES_HOST");
    string? postgresPort = await secretManager.GetSecretAsync("POSTGRES_PORT");
    string? postgresDatabase = await secretManager.GetSecretAsync("POSTGRES_DATABASE");
    string? postgresUsername = await secretManager.GetSecretAsync("POSTGRES_USERNAME");
    string? postgresPassword = await secretManager.GetSecretAsync("POSTGRES_PASSWORD");
    string? postgresConnectionString = $"Host={postgresHost};Port={postgresPort};Database={postgresDatabase};Username={postgresUsername};Password={postgresPassword}";
    options
        .UseNpgsql(postgresConnectionString)
        .LogTo(_ => { }, LogLevel.Warning);
});
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
builder.WebHost.ConfigureKestrel((_, server) => {
    string portNumber = Environment.GetEnvironmentVariable("PORT") ?? "3001";
    int port = int.Parse(portNumber);
    server.Listen(IPAddress.Any, port);
});

WebApplication app = builder.Build();
using (IServiceScope? scope = app.Services.CreateScope())
{
    ILogger<Program> logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        CollectionGalleryContext context = scope.ServiceProvider.GetRequiredService<CollectionGalleryContext>();
        context.Database.Migrate();

        // logger.LogInformation("Starting transaction...");
        // using (IDbContextTransaction? dbContext = await context.Database.BeginTransactionAsync())
        // {
        //     logger.LogInformation("Selecting with FOR UPDATE...");
        //     Model? account = await context.Models.FromSqlRaw("SELECT * FROM \"models\" WHERE \"id\" = {0} FOR UPDATE", 2).FirstAsync();
        //     logger.LogInformation("Fetched Transaction. Model Name - {0}", account.Name);
        //     account.Name = "Hello";
        //     account.UpdatedAt = DateTime.UtcNow;
        //     Thread.Sleep(5000);
        //     await context.SaveChangesAsync();
        //     await context.Database.CommitTransactionAsync();
        //     logger.LogInformation("Commited");
        // }

        // AsyncRetryPolicy? retryPolicy = Policy
        //     .Handle<HttpRequestException>()
        //     .WaitAndRetryAsync(
        //         retryCount: 5,
        //         sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // exponential backoff
        //         onRetry: (ex, ts, attempt, ctx) => logger.LogWarning($"Retry {attempt}: waiting {ts.TotalSeconds}s after error {ex.Message}")
        //     );

        // UnreliableService service = new UnreliableService();
        // await retryPolicy.ExecuteAsync(async () =>
        // {
        //     string? result = await service.GetDataAsync();
        //     logger.LogInformation(result);
        // });
    }
    catch (Exception e)
    {
        logger.LogCritical(e, "Exception at DB Migrate Setup ({0})", e.Message);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.MapControllers();
app.Run();

public class UnreliableService
{
    private static readonly Random _rnd = new Random();

    public async Task<string> GetDataAsync()
    {
        await Task.Delay(1000); // simulate latency

        // 50% failure chance
        if (_rnd.NextDouble() < 0.5) throw new HttpRequestException("Simulated network failure");

        return "✅ Success at " + DateTime.Now;
    }
}
