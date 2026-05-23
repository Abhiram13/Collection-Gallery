using System.Net;
using CollectionGallery.InfraStructure.Data.Item.Models;
using CollectionGallery.InfraStructure.Data.Services;
using CollectionGallery.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace CollectionGallery.InfraStructure.Data.Controllers;

[ApiController]
[Route("api/items")]
public class ItemController : ControllerBase
{
    private readonly ILogger<ItemController> _logger;
    private readonly ItemService _itemService;

    public ItemController(ILogger<ItemController> logger, ItemService itemService)
    {
        _logger = logger;
        _itemService = itemService;
    }

    [HttpPost]
    public async Task<IActionResult> InsertOneAsync([FromBody] ItemInsertDto payload)
    {
        SignedUrlResponseDto? response = await _itemService.InsertOneItemAsync(payload);
        
        return StatusCode(StatusCodes.Status201Created, response);
    }
}