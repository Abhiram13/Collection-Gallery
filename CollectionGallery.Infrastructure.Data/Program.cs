using Microsoft.EntityFrameworkCore;
using Dotenv;
using Google.Cloud.Diagnostics.AspNetCore3;
using Google.Cloud.Diagnostics.Common;
using CollectionGallery.InfraStructure.Data;
using CollectionGallery.InfraStructure.Data.Services;
using CollectionGallery.Shared;

EnvironmentVariables.Init();
ILoggerFactory factory;
ILogger? logger;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<CollectionGalleryContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("CollectionGalleryContext"));
});
builder.Services.AddScoped<SubscriberService>();
builder.Services.AddScoped<ModelService>();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<CollectionService>();

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
    logger = factory.CreateLogger("CollectionGallery-console");
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

WebApplication app = builder.Build();

// TODO: FIX THIS SCOPING. IT SHOULD BE SINGLETON
using (var scope = app.Services.CreateScope())
{
    try
    {
        CollectionGalleryContext context = scope.ServiceProvider.GetRequiredService<CollectionGalleryContext>();
        SubscriberService service = scope.ServiceProvider.GetRequiredService<SubscriberService>();
        await service.SubscribeAsync();
        context.Database.Migrate();
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();
