using Microsoft.AspNetCore.Mvc;
using CollectionGallery.Domain.Models.Entities;
using CollectionGallery.InfraStructure.Data.Services;
using CollectionGallery.Domain.Models.Controllers;
using System.Text.Json;

namespace CollectionGallery.InfraStructure.Data.Controllers;

[ApiController]
[Route("/api/collections")]
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
    public async Task<IActionResult> CreateAsync([FromBody] CreateCollectionDto body)
    {
        string traceId = Guid.NewGuid().ToString();
        try
        {
            Collection collection = new Collection();
            DateTime dateTime = DateTime.UtcNow;
            collection.CreatedAt = dateTime;
            collection.UpdatedAt = dateTime;
            collection.Name = body.Name;

            if (body.ParentCollectionId is not null && body.ParentCollectionId > 0)
            {
                collection.ParentCollectionId = body.ParentCollectionId;
            }

            // TODO: Sometimes collection may already exists and need to handle that response/logic here
            await _collectionService.InsertAsync(collection);
            _logger.LogInformation("{@response}", new { traceId, body, message = "Collection Added Successfully" });
            return StatusCode(201, new ApiResponse<string>
            {
                StatusCode = System.Net.HttpStatusCode.Created,
                Message = "Collection added successfully",
                TraceId = traceId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{@response}", new { message = $"Exception: {ex.Message}", traceId, body });
            return StatusCode(500, new ApiResponse<string>
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Message = "Something went wrong. Please check the logs for more details.",
                TraceId = traceId
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ParentCollectionListAsync()
    {
        string traceId = Guid.NewGuid().ToString();
        try
        {
            List<ParentCollections> parentCollections = await _collectionService.ListOfParentCollections();
            return StatusCode(200, new ApiResponse<List<ParentCollections>>
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Result = parentCollections,
                TraceId = traceId
            });
        }
        catch (Exception e)
        {
            _logger.LogError("Exception at Collection Details API. {@ExceptionDetails}", new { message = e.Message, traceId });
            return StatusCode(500);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> CollectionDetailsById([FromRoute] int id)
    {
        string traceId = Guid.NewGuid().ToString();

        try
        {
            if (id == 0)
            {
                _logger.LogWarning("Invalid Collection Id is provided. Collection Id: {@CollectinId}", id);
                return StatusCode(400, new ApiResponse<string>
                {
                    Message = $"invalid collection is provided. Collection ID: {id}",
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    TraceId = traceId
                });
            }
            CollectionDetailsById? collections = await _collectionService.CollectionsById(id);
            return Ok(new ApiResponse<CollectionDetailsById>
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Result = collections,
                TraceId = traceId
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception at Collection Details API. {@ExceptionDetails}", new { message = e.Message, traceId });
            return StatusCode(500);
        }
    }

    [HttpGet("{collectionId}/items")]
    public async Task<IActionResult> ItemsByCollectionId([FromRoute] int collectionId)
    {
        string traceId = Guid.NewGuid().ToString();

        try
        {
            if (collectionId == 0)
            {
                _logger.LogWarning("Invalid Collection Id is provided. Collection Id: {@CollectinId}", collectionId);
                return StatusCode(400, new ApiResponse<string>
                {
                    Message = $"invalid collection is provided. Collection ID: {collectionId}",
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    TraceId = traceId
                });
            }

            List<ItemsByCollectionId> items = await _collectionService.GetItemsByCollectionIdAsync(collectionId);
            return Ok(new ApiResponse<List<ItemsByCollectionId>>
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Result = items,
                TraceId = traceId,
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception at Items by Collection API. {@ExceptionDetails}", new { message = e.Message, traceId });
            return StatusCode(500);
        }
    }
    
    [HttpPost("{collectionId}/items")]
    public async Task<IActionResult> CreateItemByCollectionIdAsync([FromRoute] int collectionId, [FromForm] CreateItemByCollectionIdDto payload)
    {
        _logger.LogInformation(Request.ContentType);
        _logger.LogInformation(Request.HasFormContentType.ToString());
        _logger.LogInformation(Request.HasJsonContentType().ToString());

        _logger.LogInformation(JsonSerializer.Serialize(payload));
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] Collection body)
    {
        await _collectionService.UpdateByIdAsync(id, body);
        return Ok();
    }
}