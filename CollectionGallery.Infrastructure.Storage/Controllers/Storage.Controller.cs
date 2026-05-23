using CollectionGallery.Infrastructure.Storage.Services;
using CollectionGallery.Shared.Models;
using Google.Apis.Storage.v1;
using Microsoft.AspNetCore.Mvc;

namespace CollectionGallery.InfraStructure.Storage.Controllers;

[ApiController]
[Route("api/storage")]
public class GoogleCloudStorageController : ControllerBase
{
    private readonly GoogleStorageService _storageService;
    
    public GoogleCloudStorageController(GoogleStorageService storageService)
    {
        _storageService = storageService;
    }

    [HttpPost]
    public IActionResult GetSignedUrl([FromBody] GetSignedUrlDto request)
    {
        string url = _storageService.GenerateSignedUrl(request.FileName);
        return Ok(new SignedUrlResponseDto
        {
            SignedUrl = url
        });
    }

    [HttpGet("{fileName}")]
    public IActionResult GetObjectMetaData([FromRoute] string fileName)
    {
        return Ok(_storageService.GetObjectMetadata(fileName));
    }
}