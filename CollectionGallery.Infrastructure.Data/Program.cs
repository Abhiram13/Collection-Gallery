using System.Net;
using Microsoft.EntityFrameworkCore;
using Google.Cloud.Diagnostics.AspNetCore3;
using Google.Cloud.Diagnostics.Common;
using CollectionGallery.InfraStructure.Data;
using CollectionGallery.InfraStructure.Data.Services;
using Abhiram.Extensions.DotEnv;
using Abhiram.Abstractions.Logging;
using Abhiram.Secrets.Providers;
using CollectionGallery.InfraStructure.Data.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
DotEnvironmentVariables.Load();

builder.Configuration.AddJsonFile(Path.Combine("/secrets/", "collection-gallery-data-secrets.json"), optional: true, reloadOnChange: true);
builder.AddConsoleGoogleSeriLog();
builder.Services.AddApplicationCollections(builder.Configuration);
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
        MigrateDbContext context = scope.ServiceProvider.GetRequiredService<MigrateDbContext>();
        context.Database.Migrate();
    }
    catch (Exception e)
    {
        logger.LogCritical(e, "Exception at DB Migrate Setup ({Message})", e.Message);
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
app.UseExceptionHandler();
app.MapControllers();
app.MapGet("/", () => new { StatusCode = HttpStatusCode.OK, Message = "This is Collection gallery Data API" });
app.Run();
