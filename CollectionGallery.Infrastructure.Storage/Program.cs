using Dotenv;
using Google.Cloud.Diagnostics.AspNetCore3;
using Google.Cloud.Diagnostics.Common;
using CollectionGallery.Infrastructure.Storage.Services;
using CollectionGallery.Shared;

EnvironmentVariables.Init();
ILoggerFactory factory;
ILogger? logger;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables().Build();
builder.Logging.ClearProviders();

if (Environment.GetEnvironmentVariable("ENV") == "Development" || Environment.GetEnvironmentVariable("ENV") == "Test")
{
    builder.Logging.AddConsole();
    factory = LoggerFactory.Create(log =>
    {
        // log.AddConsole();
        log.AddSimpleConsole(options =>
        {
            options.IncludeScopes = true;
            options.SingleLine = true;
            options.TimestampFormat = "HH:mm:ss ";
            options.IncludeScopes = true;
        });
    });
    logger = factory.CreateLogger("Collection-Gallery-console");
    Logger.Initialize(logger);
}
else
{
    builder.Logging.AddGoogle();
    factory = LoggerFactory.Create(log => log.AddGoogle());
    logger = factory.CreateLogger("Google-cloud-console");
    builder.Services.AddGoogleDiagnosticsForAspNetCore();
    Logger.Initialize(logger);
}

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ItemService>();
builder.Services.AddSingleton<PublisherService>();
builder.Services.AddSingleton<ImageService>();
builder.Services.AddControllers();
WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
