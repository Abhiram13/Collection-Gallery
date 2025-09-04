using Google.Cloud.Storage.Control.V2;

namespace CollectionGallery.Infrastructure.Storage.Services;

[Obsolete]
public class CollectionService : GoogleStorageService
{
    public async Task CreateFolderAsync(string folderName)
    {
        CreateFolderRequest request = new CreateFolderRequest()
        {
            Parent = BucketName.FormatProjectBucket("_", _bucketName),
            FolderId = folderName,
        };
        await _storageControlClient.CreateFolderAsync(request);
    }

    public List<string> ListOfFolders()
    {
        string bucketName = BucketName.FormatProjectBucket("_", _bucketName);
        var folders = _storageControlClient.ListFolders(bucketName);
        List<string> folderNames = new List<string>();

        foreach (Folder folder in folders)
        {
            string name = folder.FolderName.FolderId.TrimEnd('/');
            folderNames.Add(name);
        }

        return folderNames;
    }

    public async Task DeleteFolderAsync(string folderName)
    {
        string[] names = folderName.Split(":");
        string name = string.Join('/', names);
        string folderResourceName = FolderName.FormatProjectBucketFolder("_", _bucketName, name);
        await _storageControlClient.DeleteFolderAsync(folderResourceName);
    }
}