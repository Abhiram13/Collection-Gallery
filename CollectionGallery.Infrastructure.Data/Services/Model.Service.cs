using Microsoft.EntityFrameworkCore;
using CollectionGallery.Domain.Models.Entities;

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
        await _modelDataSet.AddAsync(model);
        await _context.SaveChangesAsync();
        return model;
    }

    public async Task<List<Model>> ListAsync()
    {
        List<Model> list = await _modelDataSet.Select(m => new Model { Name = m.Name, Id = m.Id}).ToListAsync();
        return list;
    }
}