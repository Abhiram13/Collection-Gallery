using Microsoft.EntityFrameworkCore;
using Abhiram.Secrets.Providers;
using CollectionGallery.InfraStructure.Data.Services;
using System.Net;
using CollectionGallery.InfraStructure.Data.Configurations;
using CollectionGallery.InfraStructure.Data.Repository;
using CollectionGallery.InfraStructure.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CollectionGallery.InfraStructure.Data.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection collection)
    {
        public IServiceCollection AddApplicationCollections(IConfiguration configuration)
        {
            collection
                .AddOptionConfigurations(configuration)
                .AddPostgresDBContext<WriteDbContext>(DatabaseType.WRITE)
                .AddPostgresDBContext<ReadDbContext>(DatabaseType.READ)
                .AddPostgresDBContext<MigrateDbContext>(DatabaseType.MIGRATE)
                .AddDependencyServices()
                .AddOtherServices();
            
            return collection;
        }
        
        private IServiceCollection AddOtherServices()
        {
            collection.AddEndpointsApiExplorer();
            collection.AddSwaggerGen();
            collection.AddControllers();
            collection.ConfigureHttpJsonOptions(options => options.SerializerOptions.PropertyNamingPolicy = null);
            collection.AddRouting();
            collection.AddExceptionHandler<InvalidPayloadExceptionHandler>();
            collection.AddExceptionHandler<GlobalExceptionHandler>();
            collection.AddProblemDetails();

            return collection;
        }

        private IServiceCollection AddDependencyServices()
        {
            // collection.AddHostedService<SubscriberBackgroundService>();
            collection.AddScoped<SubscriberService>();
            collection.AddScoped<ItemService>();
            collection.AddScoped<CollectionService>();
            collection.AddScoped<TagService>();
            collection.AddScoped<CollectionRepository>();
            collection.AddSingleton<DataSecrets>(sp => sp.GetRequiredService<IOptions<DataSecrets>>().Value);

            return collection;
        }

        private IServiceCollection AddOptionConfigurations(IConfiguration configuration)
        {
            collection.
                ConfigureOptions<PostgresDatabaseNameConfiguration>().
                AddOptions<DataSecrets>().
                Bind(configuration).
                ValidateDataAnnotations().
                ValidateOnStart();
            
            return collection;
        }

        private IServiceCollection AddPostgresDBContext<TContext>(string dbType) where TContext : DbContext
        {
            collection.AddDbContext<TContext>((provider, options) =>
            {
                PostgresSecrets secrets = provider.GetRequiredService<IOptionsMonitor<PostgresSecrets>>().Get(dbType);
                string host = secrets.Host;
                int port = secrets.Port;
                string username  = secrets.Username;
                string password = secrets.Password;
                string database = secrets.Database;
                string connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";
                
                options.UseNpgsql(connectionString).LogTo(_ => { }, LogLevel.Warning);
            });

            return collection;
        }
    }
}