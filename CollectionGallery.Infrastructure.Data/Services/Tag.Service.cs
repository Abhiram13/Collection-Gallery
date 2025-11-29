using System.Text.Json;
using CollectionGallery.Domain.Models.Controllers;
using CollectionGallery.Domain.Models.Entities;
using CollectionGallery.Domain.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CollectionGallery.InfraStructure.Data.Services;

public class TagService : BaseService
{
    private readonly ILogger<TagService> _logger;

    public TagService(ILogger<TagService> logger, CollectionGalleryWriteContext writeContext, CollectionGalleryReadContext readContext) : base (writeContext, readContext)
    {
        _logger = logger;
    }

    public async Task<Tags> SearchAndInsertAsync(Tags tag)
    {
        Tags? existingTag = await SearchByName(tag.Name);

        if (existingTag is not null)
        {
            _logger.LogWarning("Tag ({0}) already exists. Skipping the insertion", tag.Name);
            return existingTag;
        }

        await _writeContext.Tags.AddAsync(tag);
        await _writeContext.SaveChangesAsync();
        return tag;
    }

    private async Task<Tags?> SearchByName(string platformName)
    {
        Tags? tag = await _writeContext.Tags.FirstOrDefaultAsync(p => p.Name.ToLower() == platformName.ToLower());
        return tag;
    }

    public async Task AddItemTagsAsync(int itemId, List<int>? tagIds)
    {
        if (tagIds is null || tagIds.Count == 0)
        {
            _logger.LogWarning("Tag Ids ({0}) is either empty of null. Skipping Item Tags insertion", JsonSerializer.Serialize(tagIds));
            return;
        }

        foreach (int tagId in tagIds)
        {
            if (tagId == 0) continue;

            await _writeContext.ItemTags.AddAsync(new ItemTags { ItemId = itemId, TagId = tagId });
            await _writeContext.SaveChangesAsync();
        }
    }

    public async Task<List<TagList>> ListTagsAsync()
    {
        List<TagList> list = await _writeContext.Tags.Where(t => !string.IsNullOrEmpty(t.Name)).Select(t => new TagList { Id = t.Id, Name = t.Name }).ToListAsync();
        return list;
    }

    public async Task<UpdateFieldResult> UpdateByIdAsync(int tagId, Tags body)
    {
        Tags? existingTag = await _writeContext.Tags.FindAsync(tagId);

        if (existingTag is null)
        {
            return UpdateFieldResult.NotFound;
        }

        if (!string.IsNullOrEmpty(body.Name)) existingTag.Name = body.Name;
        existingTag.UpdatedAt = DateTime.UtcNow;

        await _writeContext.SaveChangesAsync();
        return UpdateFieldResult.Success;
    }

    public async Task<bool> IsTagExistAsync(int tagId)
    {
        int tagsCount = await _writeContext.Tags.CountAsync(t => t.Id == tagId);
        return tagsCount > 0;
    }
}