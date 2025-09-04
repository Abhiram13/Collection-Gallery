using Microsoft.AspNetCore.Mvc;
using CollectionGallery.Infrastructure.Storage.Services;
using CollectionGallery.Domain.Models.Controllers;
using CollectionGallery.Infrastructure.Storage.Utilities;
using CollectionGallery.Shared;
using System.Text.Json;

namespace CollectionGallery.Infrastructure.Storage.Controllers;

[ApiController]
[Route("item")]
public class ItemController : ControllerBase
{
    private readonly ItemService _service;
    private readonly PublisherService _publisher;
    private readonly ImageService _imageService;

    public ItemController(ItemService service, PublisherService publisher, ImageService imageService)
    {
        _service = service;
        _publisher = publisher;
        _imageService = imageService;
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
                Model = uploadForm.Name
            };
            await _publisher.PublishMessageAsync(JsonSerializer.Serialize(resultObject), TRACE_ID, "FileUpload");
            return Ok(resultObject);
        }
        catch (Exception e)
        {
            ProblemDetails problemDetails = new ProblemDetails
            {
                Title = "File upload failure",
                Status = 500,
                Instance = "POST /item"
            };
            problemDetails.Detail = $"Trace ID: {TRACE_ID}. Error message: {e.Message}";
            Logger.LogError(e, problemDetails);

            problemDetails.Detail = $"Something went wrong while uploading the file. Check the logs for more details. Trace ID: {TRACE_ID}";
            return StatusCode(500, problemDetails);
        }        
    }
}