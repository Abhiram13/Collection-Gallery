using Google.Cloud.Diagnostics.AspNetCore3;
using Google.Cloud.Diagnostics.Common;
using CollectionGallery.Infrastructure.Storage.Services;
using Abhiram.Extensions.DotEnv;
using Abhiram.Abstractions.Logging;
using System.Net;
using CollectionGallery.InfraStructure.Storage.Configuration;
using Microsoft.Extensions.Options;

DotEnvironmentVariables.Load();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables().Build();
builder.AddConsoleGoogleSeriLog();
builder.Services.AddOptions<StorageSecrets>().Bind(builder.Configuration).ValidateOnStart();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// builder.Services.AddSingleton<ItemService>();
builder.Services.AddSingleton<PublisherService>();
builder.Services.AddSingleton<ImageService>();
builder.Services.AddSingleton<StorageSecrets>(sp => sp.GetRequiredService<IOptions<StorageSecrets>>().Value);
builder.Services.AddControllers();
builder.WebHost.ConfigureKestrel((_, server) => {
    string portNumber = Environment.GetEnvironmentVariable("PORT") ?? "3003";
    int port = int.Parse(portNumber);
    server.Listen(IPAddress.Any, port);
});

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
