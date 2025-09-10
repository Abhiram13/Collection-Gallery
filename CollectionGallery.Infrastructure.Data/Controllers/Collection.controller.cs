using Microsoft.AspNetCore.Mvc;
using CollectionGallery.Domain.Models.Entities;
using CollectionGallery.InfraStructure.Data.Services;
using CollectionGallery.Domain.Models.Controllers;

namespace CollectionGallery.InfraStructure.Data.Controllers;

[ApiController]
[Route("collection")]
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
    public async Task<ActionResult<ApiResponse<string>>> CreateAsync([FromBody] Collection body)
    {
        string traceId = Guid.NewGuid().ToString();
        try
        {
            DateTime dateTime = DateTime.UtcNow;
            body.CreatedAt = dateTime;
            body.UpdatedAt = dateTime;
            await _collectionService.InsertAsync(body);
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
    public async Task<ActionResult> ParentCollectionListAsync()
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
    public async Task<ActionResult<ApiResponse<CollectionDetailsById>>> CollectionDetailsById(int id)
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
            _logger.LogError("Exception at Collection Details API. {@ExceptionDetails}", new { message = e.Message, traceId });
            return StatusCode(500);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAsync(int id, [FromBody] Collection body)
    {
        await _collectionService.UpdateByIdAsync(id, body);
        return Ok();
    }
}