using System.Net;
using CollectionGallery.Domain.Models.Controllers;
using CollectionGallery.InfraStructure.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace CollectionGallery.InfraStructure.Data.Controllers;

[ApiController]
[Route("item")]
public class ItemController : ControllerBase
{
    private readonly ILogger<ItemController> _logger;
    private readonly ItemService _itemService;

    public ItemController(ILogger<ItemController> logger, ItemService itemService)
    {
        _logger = logger;
        _itemService = itemService;
    }

    [HttpGet]
    public async Task<ActionResult> ListAsync()
    {
        string traceId = Guid.NewGuid().ToString();

        try
        {
            List<ItemList> list = await _itemService.ListAsync();
            return StatusCode(200, new ApiResponse<List<ItemList>>
            {
                TraceId = traceId,
                Result = list,
                StatusCode = HttpStatusCode.OK
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse<string>
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "Something went wrong",
                TraceId = traceId
            });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        ItemDetails details = await _itemService.ItemByIdAsync(id);
        return Ok(new ApiResponse<ItemDetails>
        {
            StatusCode = HttpStatusCode.OK,
            Result = details,
            TraceId = Guid.NewGuid().ToString(),
        });
    }
}