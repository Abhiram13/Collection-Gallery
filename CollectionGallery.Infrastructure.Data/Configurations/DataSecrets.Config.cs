using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace CollectionGallery.InfraStructure.Data.Configurations;

public record DataSecrets
{
    [Required]
    [ConfigurationKeyName("GOOGLE_CLOUD_PROJECT_ID")]
    public required string GoogleProjectId { get; init; }
    
    [ConfigurationKeyName("PORT")]
    public int Port { get; init; }

    public PubSubConfig PubSub { get; init; } = default!;
    public string StorageServer { get; init; } = string.Empty;
}

public record PubSubConfig
{
    public string StorageUploadSubscription { get; init; } = string.Empty;
}

public record PostgresSecrets
{
    public required int Port { get; init; }
    public required string Host { get; init; }
    public required string Database { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
}

public static class DatabaseType
{
    public const string WRITE = "WRITE";
    public const string READ = "READ";
    public const string MIGRATE = "MIGRATE";
}

public class PostgresDatabaseNameConfiguration : IConfigureNamedOptions<PostgresSecrets>
{
    private readonly IConfiguration _configuration;

    public PostgresDatabaseNameConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public void Configure(PostgresSecrets options)
    {
        Configure(name: DatabaseType.WRITE, options);
    }

    public void Configure(string? name, PostgresSecrets options)
    {
        _configuration.GetSection($"Postgres:{name}").Bind(options);
    }
}