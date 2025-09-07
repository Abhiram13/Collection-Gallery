using Google.Cloud.Diagnostics.AspNetCore3;
using Google.Cloud.Diagnostics.Common;
using CollectionGallery.Infrastructure.Storage.Services;
using Abhiram.Extensions.DotEnv;
using Abhiram.Abstractions.Logging;

DotEnvironmentVariables.Load();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables().Build();
builder.AddConsoleGoogleSeriLog();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ItemService>();
builder.Services.AddSingleton<PublisherService>();
builder.Services.AddSingleton<ImageService>();
builder.Services.AddControllers();
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
