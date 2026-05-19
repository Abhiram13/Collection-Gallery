using System.Net;
using Microsoft.AspNetCore.Mvc;
using CollectionGallery.InfraStructure.Data.Entities;
using CollectionGallery.InfraStructure.Data.Services;
using CollectionGallery.InfraStructure.Data.Collection.Models;
using CollectionGallery.Shared.Models;

namespace CollectionGallery.InfraStructure.Data.Controllers;

[ApiController]
[Route("api/collections")]
public class CollectionController : ControllerBase
{
    private readonly ILogger<CollectionController> _logger;
    private readonly CollectionService _collectionService;

    public CollectionController(ILogger<CollectionController> logger, CollectionService collectionService)
    {
        _logger = logger;
        _collectionService = collectionService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse>> CreateAsync([FromBody] InsertCollectionDto body)
    {
        await _collectionService.InsertAsync(body);
        
        return StatusCode(StatusCodes.Status201Created, new ApiResponse
        {
            StatusCode = HttpStatusCode.Created,
            Message = "Collection created successfully"
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllParentCollectionsAsync()
    {
        IReadOnlyList<ParentCollection> parentCollections = await _collectionService.GetAllParentCollectionsAsync();
        
        return Ok(new ApiResponse<IReadOnlyList<ParentCollection>>
        {
            StatusCode = HttpStatusCode.OK,
            Result = parentCollections
        });
    }
    //
    // [HttpGet("{id}")]
    // public async Task<ActionResult<ApiResponse<CollectionDetailsById>>> CollectionDetailsById(int id)
    // {
    //     string traceId = Guid.NewGuid().ToString();
    //
    //     try
    //     {
    //         if (id == 0)
    //         {
    //             _logger.LogWarning("Invalid Collection Id is provided. Collection Id: {@CollectinId}", id);
    //             return StatusCode(400, new ApiResponse<string>
    //             {
    //                 Message = $"invalid collection is provided. Collection ID: {id}",
    //                 StatusCode = System.Net.HttpStatusCode.BadRequest,
    //                 TraceId = traceId
    //             });
    //         }
    //         CollectionDetailsById? collections = await _collectionService.CollectionsById(id);
    //         return Ok(new ApiResponse<CollectionDetailsById>
    //         {
    //             StatusCode = System.Net.HttpStatusCode.OK,
    //             Result = collections,
    //             TraceId = traceId
    //         });
    //     }
    //     catch (Exception e)
    //     {
    //         _logger.LogError("Exception at Collection Details API. {@ExceptionDetails}", new { message = e.Message, traceId });
    //         return StatusCode(500);
    //     }
    // }
    //
    // [HttpPut("{id}")]
    // public async Task<ActionResult> UpdateAsync(int id, [FromBody] CollectionEntity body)
    // {
    //     await _collectionService.UpdateByIdAsync(id, body);
    //     return Ok();
    // }
}