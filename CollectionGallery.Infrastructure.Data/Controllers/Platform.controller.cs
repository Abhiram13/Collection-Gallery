using CollectionGallery.Domain.Models.Entities;
using CollectionGallery.InfraStructure.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace CollectionGallery.InfraStructure.Data.Controllers;

[ApiController]
[Route("platform")]
public class PlatformController : ControllerBase
{
    private readonly ILogger<PlatformController> _logger;
    private readonly PlatformService _platformService;

    public PlatformController(ILogger<PlatformController> logger, PlatformService platformService)
    {
        _logger = logger;
        _platformService = platformService;
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync([FromBody] Platforms platform)
    {
        try
        {
            DateTime dateTime = DateTime.UtcNow;
            platform.CreatedAt = dateTime;
            platform.UpdatedAt = dateTime;
            Platforms result = await _platformService.SearchAndInsertAsync(platform);
            _logger.LogInformation("Platform ({0}) with Id ({1}) added successfully", platform.Name, platform.Id);
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode(500, "Something went wrong");
        }
    }
}