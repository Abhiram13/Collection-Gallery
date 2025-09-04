using CloudNative.CloudEvents;
using Google.Cloud.Functions.Framework;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Threading;
using Google.Events.Protobuf.Cloud.Storage.V1;
using Google.Cloud.Storage.V1;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Text.Json;
using StorageObject = Google.Apis.Storage.v1.Data.Object;
using System.Collections.Generic;

namespace CollectionGallery.Functions.Services;

public class EntryFunction : ICloudEventFunction<StorageObjectData>
{
    private readonly ILogger _logger;

    public EntryFunction(ILogger<EntryFunction> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(CloudEvent cloudEvent, StorageObjectData data, CancellationToken cancellationToken)
    {
        // Prevent infinite loop: output files are tagged with "processed": "true"
        // to avoid retriggering this function when they’re uploaded back to the bucket.
        const string KEY = "processed";        

        if (data.Metadata.ContainsKey(KEY))
        {
            string isProcessed = data.Metadata[KEY];

            if (!string.IsNullOrEmpty(isProcessed) && isProcessed == "true")
            {
                _logger.LogWarning("File already processed");
                return;
            }            
        }

        string sourceFileName = data.Name;
        string sourceBucket = data.Bucket;
        StorageClient storageClient = StorageClient.Create();

        using (MemoryStream stream = new MemoryStream())
        {
            await storageClient.DownloadObjectAsync(sourceBucket, sourceFileName, stream);
            _logger.LogInformation("Object was downloaded");
            stream.Position = 0;

            using (Image image = await Image.LoadAsync(stream))
            {
                float percentage = 0.5f;
                int newWidth = (int)(image.Width * percentage);
                int newHeight = (int)(image.Height * percentage);

                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(newWidth, newHeight),
                    Mode = ResizeMode.Max
                }));

                using (Stream inputStream = new MemoryStream())
                {
                    await image.SaveAsPngAsync(inputStream);
                    inputStream.Position = 0;

                    StorageObject obj = new StorageObject
                    {
                        Bucket = sourceBucket,
                        Name = "resized-image-from-function.png",
                        ContentType = data.ContentType,
                        Metadata = new Dictionary<string, string> { { "processed", "true" } }
                    };

                    await storageClient.UploadObjectAsync(destination: obj, source: inputStream);
                    _logger.LogInformation("File successfully uploaded");
                }
            }
        }

        // await new PublisherService().PublishMessageAsync("This is from Google cloud functions through Google Pub/Sub :)");

        Console.WriteLine($"A new file has been uploaded.");
        _logger.LogInformation($"Bucket: {sourceBucket}");
        _logger.LogInformation($"File: {sourceFileName}");
        _logger.LogInformation("JSON DATA {0}", JsonSerializer.Serialize(data));

        // return Task.CompletedTask;
    }
}