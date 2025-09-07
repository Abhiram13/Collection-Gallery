using Microsoft.EntityFrameworkCore;
using Google.Cloud.Diagnostics.AspNetCore3;
using Google.Cloud.Diagnostics.Common;
using CollectionGallery.InfraStructure.Data;
using CollectionGallery.InfraStructure.Data.Services;
using Abhiram.Extensions.DotEnv;
using Abhiram.Abstractions.Logging;
using System.Net;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
DotEnvironmentVariables.Load();

builder.AddConsoleGoogleSeriLog();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<CollectionGalleryContext>(options =>
{
    string? postgresHost = Environment.GetEnvironmentVariable("POSTGRES_HOST");
    string? postgresPort = Environment.GetEnvironmentVariable("POSTGRES_PORT");
    string? postgresDatabase = Environment.GetEnvironmentVariable("POSTGRES_DATABASE");
    string? postgresUsername = Environment.GetEnvironmentVariable("POSTGRES_USERNAME");
    string? postgresPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
    string? postgresConnectionString = $"Host={postgresHost};Port={postgresPort};Database={postgresDatabase};Username={postgresUsername};Password={postgresPassword}";
    options
        .UseNpgsql(postgresConnectionString)
        .LogTo(_ => { }, LogLevel.Warning);
});
builder.Services.AddControllers();
builder.Services.AddRouting();
builder.Services.AddScoped<SubscriberService>();
builder.Services.AddScoped<ModelService>();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<CollectionService>();
builder.Services.AddScoped<PlatformService>();
builder.Services.AddScoped<TagService>();

WebApplication app = builder.Build();

// TODO: FIX THIS SCOPING. IT SHOULD BE SINGLETON
using (var scope = app.Services.CreateScope())
{
    try
    {
        CollectionGalleryContext context = scope.ServiceProvider.GetRequiredService<CollectionGalleryContext>();
        SubscriberService service = scope.ServiceProvider.GetRequiredService<SubscriberService>();
        // await service.SubscribeAsync(); // TODO: This is blocking the server running state
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
app.MapControllers();
app.Run();
