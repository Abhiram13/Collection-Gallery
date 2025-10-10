using Google.Cloud.Diagnostics.AspNetCore3;
using Google.Cloud.Diagnostics.Common;
using CollectionGallery.Infrastructure.Storage.Services;
using Abhiram.Extensions.DotEnv;
using Abhiram.Abstractions.Logging;
using System.Net;

DotEnvironmentVariables.Load();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables().Build();
builder.AddConsoleGoogleSeriLog();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ItemService>();
builder.Services.AddSingleton<PublisherService>();
builder.Services.AddSingleton<ImageService>();
builder.Services.AddControllers();
builder.WebHost.ConfigureKestrel((_, server) => {
    string portNumber = Environment.GetEnvironmentVariable("PORT") ?? "3000";
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
