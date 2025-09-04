using Microsoft.AspNetCore.Mvc;
using CollectionGallery.Infrastructure.Storage.Services;
using CollectionGallery.Domain.Models.Controllers;
using CollectionGallery.Infrastructure.Storage.Utilities;

namespace CollectionGallery.Infrastructure.Storage.Controllers;

[ApiController]
[Route("item")]
public class ItemController : ControllerBase
{
    private readonly ItemService _service;
    private readonly PublisherService _publisher;
    private readonly ImageService _imageService;

    public ItemController(ItemService service, PublisherService publisher, ImageService imageService)
    {
        _service = service;
        _publisher = publisher;
        _imageService = imageService;
    }

    [HttpPost]
    public async Task<ActionResult<StorageObject>> UploadFileAsync([FromForm] FileUploadForm form)
    {
        string TRACE_ID = Guid.NewGuid().ToString();

        FileMeta meta = new FileMeta(form.File);
        StorageObject obj = await _service.UploadFileAsync(meta);

        return Ok();
    }
}