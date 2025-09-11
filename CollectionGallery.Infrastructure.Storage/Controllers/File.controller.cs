using Microsoft.AspNetCore.Mvc;
using CollectionGallery.Infrastructure.Storage.Services;
using CollectionGallery.Domain.Models.Controllers;
using CollectionGallery.Infrastructure.Storage.Utilities;
using System.Text.Json;

namespace CollectionGallery.Infrastructure.Storage.Controllers;

[ApiController]
[Route("")]
public class FileController : ControllerBase
{
    private readonly ItemService _service;
    private readonly ILogger<FileController> _logger;

    public FileController(ItemService service, ILogger<FileController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet("{fileName}")]
    public async Task<ActionResult> GetFile(string fileName)
    {
        try
        {
            if (string.IsNullOrEmpty(fileName)) return BadRequest();
            
            FileStreamResult result = await _service.GetFileAsync(fileName);
            
            Response.Headers["Cache-Control"] = "public, max-age=31536000";

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode(500);
        }
    }
}