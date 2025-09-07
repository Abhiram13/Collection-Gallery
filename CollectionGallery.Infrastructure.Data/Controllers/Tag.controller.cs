using CollectionGallery.Domain.Models.Entities;
using CollectionGallery.InfraStructure.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace CollectionGallery.InfraStructure.Data.Controllers;

[ApiController]
[Route("tag")]
public class TagController : ControllerBase
{
    private readonly ILogger<TagController> _logger;
    private readonly TagService _tagService;

    public TagController(ILogger<TagController> logger, TagService tagService)
    {
        _logger = logger;
        _tagService = tagService;
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync([FromBody] Tags tag)
    {
        try
        {
            DateTime dateTime = DateTime.UtcNow;
            tag.CreatedAt = dateTime;
            tag.UpdatedAt = dateTime;
            Tags result = await _tagService.SearchAndInsertAsync(tag);
            _logger.LogInformation("Tag ({0}) with Id ({1}) added successfully", tag.Name, tag.Id);
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode(500, "Something went wrong");
        }
    }
}