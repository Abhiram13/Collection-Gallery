using System.Text.Json;
using CollectionGallery.Domain.Models.Controllers;
using CollectionGallery.Domain.Models.Entities;
using CollectionGallery.Domain.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CollectionGallery.InfraStructure.Data.Services;

public class TagService
{
    private readonly ILogger<TagService> _logger;
    private readonly DbSet<Tags> _tagDbSet;
    private readonly DbSet<ItemTags> _itemTagDbSet;
    private readonly CollectionGalleryContext _context;

    public TagService(ILogger<TagService> logger, CollectionGalleryContext context)
    {
        _logger = logger;
        _context = context;
        _tagDbSet = context.Tags;
        _itemTagDbSet = context.ItemTags;
    }

    public async Task<Tags> SearchAndInsertAsync(Tags tag)
    {
        Tags? existingTag = await SearchByName(tag.Name);

        if (existingTag is not null)
        {
            _logger.LogWarning("Tag ({0}) already exists. Skipping the insertion", tag.Name);
            return existingTag;
        }

        await _tagDbSet.AddAsync(tag);
        await _context.SaveChangesAsync();
        return tag;
    }

    private async Task<Tags?> SearchByName(string platformName)
    {
        Tags? tag = await _tagDbSet.FirstOrDefaultAsync(p => p.Name.ToLower() == platformName.ToLower());
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

            await _itemTagDbSet.AddAsync(new ItemTags { ItemId = itemId, TagId = tagId });
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<TagList>> ListTagsAsync()
    {
        List<TagList> list = await _tagDbSet.Where(t => !string.IsNullOrEmpty(t.Name)).Select(t => new TagList { Id = t.Id, Name = t.Name }).ToListAsync();
        return list;
    }

    public async Task<UpdateFieldResult> UpdateByIdAsync(int tagId, Tags body)
    {
        Tags? existingTag = await _tagDbSet.FindAsync(tagId);

        if (existingTag is null)
        {
            return UpdateFieldResult.NotFound;
        }

        if (!string.IsNullOrEmpty(body.Name)) existingTag.Name = body.Name;
        existingTag.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return UpdateFieldResult.Success;
    }
}