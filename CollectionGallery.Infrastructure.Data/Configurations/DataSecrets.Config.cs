using System.ComponentModel.DataAnnotations;

namespace CollectionGallery.InfraStructure.Data.Configurations;

public record DataSecrets
{
    [Required]
    [ConfigurationKeyName("Postgres")]
    public required PostgresSecrets Postgres { get; init; }
}

public record PostgresSecrets
{
    public required string Host { get; init; }
    public required int WritePort { get; init; }
    public required int ReadPort { get; init; }
    public required string Database { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
}