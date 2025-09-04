using CollectionGallery.Domain.Models.Controllers;
using CollectionGallery.Domain.Models.Entities;
using CollectionGallery.Domain.Models.Enums;
using CollectionGallery.Shared;

namespace CollectionGallery.InfraStructure.Data.Services;

public class FileService
{
    private readonly CollectionGalleryContext _context;
    private readonly ModelService _modelService;
    private readonly CollectionService _folderService;

    public FileService(CollectionGalleryContext context, ModelService service, CollectionService folderService)
    {
        _context = context;
        _modelService = service;
        _folderService = folderService;
    }

    public async Task InsertFileAsync(FileUploadResultObject data)
    {
        try
        {
            DateTime dateTime = DateTime.UtcNow;

            // First with ModelName, insert in model table and get the ID
            Model model = await _modelService.InsertAsync(new Model { Name = data.ModelName, CreatedAt = dateTime, UpdatedAt = dateTime });

            // Second with folderpaths, insert in Folders table and get the ID
            List<(int? folderId, FileSize size)> folders = await _folderService.GetOrSetFolderHierachyAsync(data, dateTime);

            // Third with model ID and folder Id, insert in the files table
            foreach (var folder in folders)
            {
                await _context.Items.AddAsync(new Item
                {
                    Extension = data.Extension,
                    CreatedAt = dateTime,
                    UpdatedAt = dateTime,
                    ModelId = model.Id,
                    ParentCollectionId = folder.folderId,
                    Size = folder.size,
                    Name = data.FileName,
                });
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, e.Message);
            throw;
        }
        // DatabaseFacade database = _context.Database;
        

        // try
        // {
        //     using (await database.BeginTransactionAsync())
        //     {
        //         Models model = await _modelService.SearchByName(data.ModelName);

            //         if (model.Id is 0)
            //         {
            //             model = await _modelService.InsertAsync(new Models { Name = data.ModelName, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
            //         }

            //         await _context.Files.AddAsync(new Files
            //         {
            //             CreatedAt = DateTime.UtcNow,
            //             Extension = data.OriginalUrl,
            //             Large = "",
            //             Medium = "",
            //             Name = "image-name",
            //             Original = data.OriginalUrl,
            //             UpdatedAt = DateTime.UtcNow,
            //             Small = "",
            //             ModelId = model.Id,
            //         });
            //         await _context.SaveChangesAsync();
            //         await database.CommitTransactionAsync();
            //     }
            // }
            // catch (Exception ex)
            // {
            //     Console.WriteLine(ex.Message);
            //     await database.RollbackTransactionAsync();
            // }        
    }
}