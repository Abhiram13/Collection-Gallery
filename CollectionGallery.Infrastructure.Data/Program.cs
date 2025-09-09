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
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<CollectionService>();
builder.Services.AddScoped<PlatformService>();
builder.Services.AddScoped<TagService>();
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

WebApplication app = builder.Build();
using (IServiceScope? scope = app.Services.CreateScope())
{
    ILogger<Program> logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        CollectionGalleryContext context = scope.ServiceProvider.GetRequiredService<CollectionGalleryContext>();
        context.Database.Migrate();
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
app.MapControllers();
app.Run();
