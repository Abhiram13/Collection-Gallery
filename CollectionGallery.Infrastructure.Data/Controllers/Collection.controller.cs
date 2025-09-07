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
    public async Task<ActionResult> CreateAsync([FromBody] Collection body)
    {
        try
        {
            DateTime dateTime = DateTime.UtcNow;
            body.CreatedAt = dateTime;
            body.UpdatedAt = dateTime;
            await _collectionService.InsertAsync(body);
            _logger.LogInformation("Collection added successfully");
            return Ok("Collection added successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, "Something went wrong");
        }
    }

    [HttpGet]
    public async Task<ActionResult> ParentCollectionListAsync()
    {
        List<ParentCollections> parentCollections = await _collectionService.ListOfParentCollections();
        return Ok(parentCollections);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> CollectionDetailsById(int id)
    {
        CollectionDetailsById? collections = await _collectionService.CollectionsById(id);
        return Ok(collections);
    }
}