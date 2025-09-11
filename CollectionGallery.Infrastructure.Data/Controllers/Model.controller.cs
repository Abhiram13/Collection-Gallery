using System.Net;
using CollectionGallery.Domain.Models.Controllers;
using CollectionGallery.Domain.Models.Entities;
using CollectionGallery.InfraStructure.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace CollectionGallery.InfraStructure.Data.Controllers;

[ApiController]
[Route("model")]
public class ModelController : ControllerBase
{
    private readonly ILogger<ModelController> _logger;
    private readonly ModelService _modelService;

    public ModelController(ILogger<ModelController> logger, ModelService modelService)
    {
        _logger = logger;
        _modelService = modelService;
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync([FromBody] Model body)
    {
        string traceId = Guid.NewGuid().ToString();
        
        try
        {
            Model? existingOne = await _modelService.SearchByName(body.Name);

            if (existingOne is not null)
            {
                StatusCode(302, new ApiResponse<string>
                {
                    StatusCode = HttpStatusCode.AlreadyReported,
                    Message = "Model already exists",
                    TraceId = traceId
                });
            }
            
            DateTime now = DateTime.UtcNow;
            body.CreatedAt = now;
            body.UpdatedAt = now;
            await _modelService.InsertAsync(body);
            return StatusCode((int) HttpStatusCode.Created, new ApiResponse<string>
            {
                StatusCode = HttpStatusCode.Created,
                Message = "Model created successfully",
                TraceId = traceId
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode((int) HttpStatusCode.InternalServerError, new ApiResponse<string>
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "Something went wrong",
                TraceId = traceId
            });
        }
    }

    [HttpGet]
    public async Task<ActionResult> ListAsync()
    {
        string traceId = Guid.NewGuid().ToString();
        
        try
        {
            List<Model>? models = await _modelService.ListAsync();
            return StatusCode(200, new ApiResponse<List<Model>>
            {
                TraceId = traceId,
                Result = models,
                StatusCode = HttpStatusCode.OK
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode((int) HttpStatusCode.InternalServerError, new ApiResponse<string>
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "Something went wrong",
                TraceId = traceId
            });
        }
    }
}