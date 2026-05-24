using System.Net;
using CollectionGallery.InfraStructure.Data.Services;
using CollectionGallery.InfraStructure.Data.Entities;
using CollectionGallery.InfraStructure.Data.Tag.Models;
using CollectionGallery.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace CollectionGallery.InfraStructure.Data.Controllers;

[ApiController]
[Route("api/tags")]
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
    public async Task<IActionResult> InsertAsync([FromBody] InsertTagDto dto)
    {
        bool isTagInserted = await _tagService.InsertAsync(dto.Name);

        if (isTagInserted)
        {
            return StatusCode(StatusCodes.Status201Created, new ApiResponse
            {
                Message = "Tag(s) inserted successfully",
                StatusCode = HttpStatusCode.Created
            });
        }
        
        return StatusCode(StatusCodes.Status304NotModified, new ApiResponse
        {
            Message = "Tag(s) already exists",
            StatusCode = HttpStatusCode.NotModified
        });
    }
}