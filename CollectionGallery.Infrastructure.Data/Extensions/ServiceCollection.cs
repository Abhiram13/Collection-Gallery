using Microsoft.EntityFrameworkCore;
using Abhiram.Secrets.Providers;
using Abhiram.Secrets.Providers.Interface;
using CollectionGallery.InfraStructure.Data.Services;
using System.Net;

namespace CollectionGallery.InfraStructure.Data.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection collection)
    {
        collection.AddEndpointsApiExplorer();
        collection.AddSwaggerGen();
        collection.AddControllers();
        collection.AddRouting();
        collection.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader())); 

        return collection;
    }

    public static IServiceCollection AddDependencyServices(this IServiceCollection collection)
    {
        collection.AddHostedService<SubscriberBackgroundService>();
        collection.AddScoped<ISecretManager, SecretManagerService>();
        collection.AddScoped<SubscriberService>();
        collection.AddScoped<ModelService>();
        collection.AddScoped<ItemService>();
        collection.AddScoped<CollectionService>();
        collection.AddScoped<PlatformService>();
        collection.AddScoped<TagService>();

        return collection;
    }

    public static IServiceCollection AddDBContext(this IServiceCollection collection)
    {
        collection.AddDbContext<CollectionGalleryContext>(async (provider, options) =>
        {
            ISecretManager secretManager = provider.GetRequiredService<ISecretManager>();
            string? postgresHost = await secretManager.GetSecretAsync("POSTGRES_HOST");
            string? postgresPort = await secretManager.GetSecretAsync("POSTGRES_PORT");
            string? postgresDatabase = await secretManager.GetSecretAsync("POSTGRES_DATABASE");
            string? postgresUsername = await secretManager.GetSecretAsync("POSTGRES_USERNAME");
            string? postgresPassword = await secretManager.GetSecretAsync("POSTGRES_PASSWORD");
            string? postgresReadDataBase = await secretManager.GetSecretAsync("POSTGRES_READ_DATABASE");
            string? postgresConnectionString = $"Host={postgresHost};Port={postgresPort};Database={postgresDatabase};Username={postgresUsername};Password={postgresPassword}";
            string? postgresReadConnectionString = $"Host={postgresHost};Port={postgresPort};Database={postgresReadDataBase};Username={postgresUsername};Password={postgresPassword}";

            options.UseNpgsql(postgresConnectionString).LogTo(_ => { }, LogLevel.Warning);
        });

        return collection;
    }
    
    public static WebApplicationBuilder AddHosting(this WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel((_, server) =>
        {
            string portNumber = Environment.GetEnvironmentVariable("PORT") ?? "3001";
            int port = int.Parse(portNumber);
            server.Listen(IPAddress.Any, port);
        });
        return builder;
    }
}