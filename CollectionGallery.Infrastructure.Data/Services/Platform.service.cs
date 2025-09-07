using System.Text.Json;
using CollectionGallery.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CollectionGallery.InfraStructure.Data.Services;

public class PlatformService
{
    private readonly ILogger<PlatformService> _logger;
    private readonly DbSet<Platforms> _platformDbSet;
    private readonly DbSet<ItemPlatforms> _itemPlatforms;
    private readonly CollectionGalleryContext _context;

    public PlatformService(ILogger<PlatformService> logger, CollectionGalleryContext context)
    {
        _logger = logger;
        _context = context;
        _platformDbSet = context.Platforms;
        _itemPlatforms = context.ItemPlatforms;
    }

    public async Task<Platforms> SearchAndInsertAsync(Platforms platform)
    {
        Platforms? existingPlatform = await SearchByName(platform.Name);

        if (existingPlatform is not null)
        {
            _logger.LogWarning("Platform ({0}) already exists. Skipping the insertion", platform.Name);
            return existingPlatform;
        }

        await _platformDbSet.AddAsync(platform);
        await _context.SaveChangesAsync();
        return platform;
    }

    private async Task<Platforms?> SearchByName(string platformName)
    {
        Platforms? platform = await _platformDbSet.FirstOrDefaultAsync(p => p.Name.ToLower() == platformName.ToLower());
        return platform;
    }
    
    public async Task AddPlatformTagsAsync(int itemId, List<int>? platformIds)
    {
        if (platformIds is null || platformIds.Count == 0)
        {
            _logger.LogWarning("Platform Ids ({0}) is either empty of null. Skipping Item Platforms insertion", JsonSerializer.Serialize(platformIds));
            return;
        }
        
        foreach (int platformId in platformIds)
        {
            if (platformId == 0) continue;
            
            await _itemPlatforms.AddAsync(new ItemPlatforms { ItemId = itemId, PlatformId = platformId });
            await _context.SaveChangesAsync();
        }
    }
}