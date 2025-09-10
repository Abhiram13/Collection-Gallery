using CollectionGallery.Domain.Models.Controllers;
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
        string traceId = Guid.NewGuid().ToString();

        try
        {
            DateTime dateTime = DateTime.UtcNow;
            tag.CreatedAt = dateTime;
            tag.UpdatedAt = dateTime;
            Tags result = await _tagService.SearchAndInsertAsync(tag);
            _logger.LogInformation("Tag ({0}) with Id ({1}) added successfully", tag.Name, tag.Id);
            return StatusCode(201, new ApiResponse<string>
            {
                StatusCode = System.Net.HttpStatusCode.Created,
                TraceId = traceId,
                Message = $"Tag ({tag.Name}) with Id ({tag.Id}) added successfully"
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode(500, new ApiResponse<string>
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                TraceId = traceId,
                Message = "Something went wrong"
            });
        }
    }

    [HttpGet]
    public async Task<ActionResult> ListAsync()
    {
        List<TagList> tags = await _tagService.ListTagsAsync();
        return StatusCode(200, tags);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateByIdAsync(int tagId, Tags body)
    {
        await _tagService.UpdateByIdAsync(tagId, body);
        return Ok();
    }
}