using Microsoft.AspNetCore.Mvc;
using CollectionGallery.Infrastructure.Storage.Services;
using CollectionGallery.Domain.Models.Controllers;
using CollectionGallery.Infrastructure.Storage.Utilities;
using System.Text.Json;
using Google;
using CollectionGallery.Domain.Models.Services;

namespace CollectionGallery.Infrastructure.Storage.Controllers;

[ApiController]
[Route("file")]
public class FileController : ControllerBase
{
    private readonly ItemService _service;
    private readonly ILogger<FileController> _logger;
    private readonly DbServiceClient _dbServiceClient;

    public FileController(ItemService service, ILogger<FileController> logger, DbServiceClient dbServiceClient)
    {
        _service = service;
        _logger = logger;
        _dbServiceClient = dbServiceClient;
    }

    [HttpGet("{fileName}")]
    public async Task<ActionResult> GetFileAsync(string fileName)
    {
        try
        {
            if (string.IsNullOrEmpty(fileName)) return BadRequest();

            FileStreamResult result = await _service.GetFileAsync(fileName);
            Response.Headers["Cache-Control"] = "public, max-age=31536000";
            return result;
        }
        catch (GoogleApiException gex)
        {
            _logger.LogError(gex, gex.Message);
            if (gex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            return StatusCode(500);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode(500);
        }
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFileAsync([FromForm] CreateItemByCollectionIdDto payload)
    {
        try
        {
            int[] tags = payload.Tags.Select(t => {
                int value;
                return int.TryParse(t, out value) ? value : 0;
            }).ToArray();
            ValidateUploadDto dto = new ValidateUploadDto { CollectionId = payload.CollectionId, Tags = tags };
            await _dbServiceClient.ValidateUploadAsync(dto);
            FileMeta meta = new FileMeta(payload.File);
            StorageObject storageObject = await _service.UploadFileAsync(meta);
            return Ok(storageObject);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(exception: ex, message: ex.Message);
            return BadRequest(new ExceptionMessage { Message = ex.Message });
        }
    }
}