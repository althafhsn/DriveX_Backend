using DriveX_Backend.DB;
using DriveX_Backend.Entities.Cars;
using DriveX_Backend.IRepository;
using Microsoft.EntityFrameworkCore;

namespace DriveX_Backend.Repository
{
    public class ModelRepository : IModelRepository
    {
        private AppDbContext _context;

        public ModelRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Model>> GetByBrandIdAsync(Guid brandId) =>
                await _context.Models.Where(m => m.BrandId == brandId).ToListAsync();
        public async Task<Model> GetByNameAndBrandIdAsync(Guid brandId, string modelName)
        {
            return await _context.Models.FirstOrDefaultAsync(m => m.BrandId == brandId && m.Name.ToLower() == modelName.ToLower());
        }

        public async Task<Model> GetByIdAsync(Guid modelId)
        {
            return await _context.Models.FirstOrDefaultAsync(m => m.Id == modelId);
        }

        public async Task<Model> AddModelAsync(Model model)
        {
            _context.Models.Add(model);
            await _context.SaveChangesAsync();
            return model;
        }
        public async Task<bool> ExistsAsync(Guid brandId, string modelName) =>
       await _context.Models.AnyAsync(m => m.BrandId == brandId && m.Name.ToLower() == modelName.ToLower());
    }
}
