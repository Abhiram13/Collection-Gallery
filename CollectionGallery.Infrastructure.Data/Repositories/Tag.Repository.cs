using CollectionGallery.InfraStructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CollectionGallery.InfraStructure.Data.Repository;

public class TagRepository : BaseRepository
{
    public TagRepository(WriteDbContext writeDbContext, ReadDbContext readDbContext) : base(writeDbContext, readDbContext) { }
    
    /// <summary>
    /// Insert a single or multiple tags at once in DB.
    /// </summary>
    /// <param name="tags">A default lowercased list of single or multiple tags to be inserted.</param>
    /// <remarks>First verifies which tag(s) already exists in DB, then exclude them from the given <paramref name="tags"/>. If any new tag(s) prssents, it gets inserted in DB.</remarks>
    public async Task InsertTagsAsync(List<string> tags)
    {
        List<string> existingTags = await _readDbContext.Tags
            .Where(t => tags.Contains(t.Name.ToLower()))
            .Select(t => t.Name)
            .ToListAsync();
        
        List<TagEntity> newTags = tags
            .Except(existingTags, StringComparer.OrdinalIgnoreCase)
            .Select(t => TagEntity.Create(t))
            .ToList();

        if (newTags.Count > 0)
        {
            await _writeDbContext.Tags.AddRangeAsync(newTags);
            await _writeDbContext.SaveChangesAsync();
        }
    }

    // /// <summary>
    // /// Checks tag in DB and returns true if exists.
    // /// </summary>
    // /// <param name="tags">List of the tags to check.</param>
    // /// <returns><c>true</c> if a tag with give name exists. <c>false</c> if tag with given name does not exists.</returns>
    // /// <remarks>The given <paramref name="tagName"/> will be trimmed of whitespaces and changed to lower case before verifying in DB.</remarks>
    // public async Task<bool> IsTagExistsAsync(List<string> tags)
    // {
    //     string trimmed = tagName.Trim().ToLower();
    //     Tags? tag = await _readDbContext.Tags.FirstOrDefaultAsync(t => t.Name == trimmed);
    //
    //     return tag is not null;
    // }
}