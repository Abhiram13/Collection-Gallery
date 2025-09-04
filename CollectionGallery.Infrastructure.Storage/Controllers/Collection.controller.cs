using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using CollectionGallery.Infrastructure.Storage.Services;
using CollectionGallery.Domain.Models.Controllers;
using CollectionGallery.Shared;

namespace Storage.Controllers;

[ApiController]
[Route("collection")]
public class CollectionController : ControllerBase
{
    private CollectionService _service { get; init; }

    public CollectionController(CollectionService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateFolderAsync([FromBody] FolderCreateForm form)
    {
        try
        {
            await _service.CreateFolderAsync(form.Name);
            Logger.LogInformation($"Folder ({form.Name}) was successfully created");
            return Ok("Folder created successfully");
        }
        catch (Grpc.Core.RpcException ex)
        {
            Logger.LogError(ex, ex.Message);

            if (ex.StatusCode == Grpc.Core.StatusCode.AlreadyExists)
            {
                return BadRequest("Folder aleady exists");
            }

            return StatusCode(500, "Something went wrong. Please check the logs");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            return StatusCode(500, "Something went wrong. Please check the logs");
        }
    }

    [HttpDelete("{folderName}")]
    public async Task<IActionResult> DeleteFolderAsync(string folderName)
    {
        await _service.DeleteFolderAsync(folderName);
        return Ok("Deleted");
    }
}