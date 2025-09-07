using Microsoft.AspNetCore.Mvc;
using CollectionGallery.Infrastructure.Storage.Services;
using CollectionGallery.Domain.Models.Controllers;
using CollectionGallery.Infrastructure.Storage.Utilities;
using System.Text.Json;

namespace CollectionGallery.Infrastructure.Storage.Controllers;

[ApiController]
[Route("item")]
public class ItemController : ControllerBase
{
    private readonly ItemService _service;
    private readonly PublisherService _publisher;
    private readonly ImageService _imageService;
    private readonly ILogger<ItemController> _logger;

    public ItemController(ItemService service, PublisherService publisher, ImageService imageService, ILogger<ItemController> logger)
    {
        _service = service;
        _publisher = publisher;
        _imageService = imageService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<StorageObject>> UploadFileAsync([FromForm] FileUploadForm uploadForm)
    {
        string TRACE_ID = Guid.NewGuid().ToString();
        try
        {
            FileMeta meta = new FileMeta(uploadForm.File);
            await _service.UploadFileAsync(meta);
            FileUploadResultObject resultObject = new FileUploadResultObject
            {
                CollectionId = uploadForm.CollectionId ?? 0,
                ContentType = meta.ContentType,
                Extension = meta.Extension,
                FileName = meta.FileName,
                TraceId = TRACE_ID,
                Model = uploadForm.Name,
                Platforms = uploadForm.Platforms,
                Tags = uploadForm.Tags
            };
            await _publisher.PublishMessageAsync(JsonSerializer.Serialize(resultObject), TRACE_ID, "FileUpload");
            return Ok(resultObject);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Something went wrong");
            ProblemDetails problemDetails = new ProblemDetails
            {
                Title = "File upload failure",
                Status = 500,
                Instance = "POST /item",
                Detail = $"Something went wrong while uploading the file. Check the logs for more details. Trace ID: {TRACE_ID}"
            };
            return StatusCode(500, problemDetails);
        }        
    }
}