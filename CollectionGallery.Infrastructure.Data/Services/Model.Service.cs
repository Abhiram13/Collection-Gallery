using Microsoft.EntityFrameworkCore;
using CollectionGallery.Domain.Models.Entities;
using CollectionGallery.Shared;

namespace CollectionGallery.InfraStructure.Data.Services;

public class ModelService
{
    private readonly CollectionGalleryContext _context;
    private readonly DbSet<Model> _modelDataSet;

    public ModelService(CollectionGalleryContext context)
    {
        _context = context;
        _modelDataSet = _context.Models;
    }

    public async Task<Model?> SearchByName(string modelName)
    {
        Model? model = await _modelDataSet.Where(m => m.Name.ToLower() == modelName.ToLower()).FirstOrDefaultAsync();
        return model;
    }

    public async Task<Model> InsertAsync(Model model)
    {
        Model? existingModel = await SearchByName(model.Name);

        if (existingModel is not null)
        {
            Logger.LogWarning($"Skipping model creation. Model '{model.Name}' entry already exists.");
            return existingModel;
        }
        
        await _modelDataSet.AddAsync(model);
        await _context.SaveChangesAsync();
        return model;
    }
}