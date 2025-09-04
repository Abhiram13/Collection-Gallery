using Microsoft.EntityFrameworkCore;
using CollectionGallery.Domain.Models.Entities;
using CollectionGallery.Domain.Models.Enums;
using CollectionGallery.Domain.Models.Controllers;
using CollectionGallery.Shared;

namespace CollectionGallery.InfraStructure.Data.Services;

public class CollectionService
{
    private readonly CollectionGalleryContext _context;
    private readonly DbSet<Collection> _collectionDataSet;
    private DateTime _dateTime;

    public CollectionService(CollectionGalleryContext context)
    {
        _context = context;
        _collectionDataSet = _context.Collections;
    }

    public async Task<Collection> InsertAsync(Collection folder)
    {
        await _collectionDataSet.AddAsync(folder);
        await _context.SaveChangesAsync();
        return folder;
    }

    private async Task<Collection> GetOrSetFolders(string collectionName, int? parentCollectionId = null)
    {
        try
        {
            System.Linq.Expressions.Expression<Func<Collection, bool>> predicate = parentCollectionId is null
                ? f => f.Name.ToLower() == collectionName.ToLower()
                : f => f.Name.ToLower() == collectionName.ToLower() && f.ParentCollectionId == parentCollectionId;

            Collection? collection = await _collectionDataSet.Where(predicate).FirstOrDefaultAsync();
            collection ??= await InsertAsync(new Collection { Name = collectionName, CreatedAt = _dateTime, UpdatedAt = _dateTime, ParentCollectionId = parentCollectionId });
            return collection;
        }
        catch (Exception e)
        {
            Logger.LogError(e, e.Message);
            throw;
        }
    }

    /// <summary>
    /// Gets or sets the folder hierarchy in the database based on the file paths in the provided result object.
    /// </summary>
    /// <param name="resultObject">The object containing the file paths (original, medium, and small) to process.</param>
    /// <param name="utcNow">The current UTC timestamp to be used for date-related operations.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task<List<(int? parentFolderId, FileSize size)>> GetOrSetFolderHierachyAsync(FileUploadResultObject resultObject, DateTime utcNow)
    {
        _dateTime = utcNow;
        List<(string path, FileSize size)> urls = new List<(string, FileSize)> {
            // (resultObject.OriginalFilePath, FileSize.Original),
            // (resultObject.MediumFilePath, FileSize.Medium),
            // (resultObject.SmallFilePath, FileSize.Small),
        };

        List<(int? parentFolderId, FileSize size)> result = new List<(int? parentFolderId, FileSize size)>();

        Logger.LogInformation("Hello");

        foreach ((string path, FileSize size) url in urls)
        {
            Logger.LogInformation(url.path);
            string? directory = Path.GetDirectoryName(url.path) ?? "";
            string[] folders = directory.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            int? parentFolderId = null;

            foreach (string folder in folders)
            {
                Collection insertResult = await GetOrSetFolders(folder, parentFolderId);
                parentFolderId = insertResult.Id;
            }

            Logger.LogInformation(result);
            if (!result.Contains((parentFolderId, url.size)))
            {
                result.Add((parentFolderId, url.size));
            }
        }

        return result;
    }
}