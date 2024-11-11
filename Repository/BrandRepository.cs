using DriveX_Backend.DB;
using DriveX_Backend.Entities.Cars;
using DriveX_Backend.IRepository;
using Microsoft.EntityFrameworkCore;

namespace DriveX_Backend.Repository
{
    public class BrandRepository:IBrandRepository
    {
        private readonly AppDbContext _context;
        public BrandRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<string>> GetModelNamesByBrandNameAsync(string brandName)
        {
            return await _context.Models
                .Where(m => m.Brand.Name == brandName)
                .Select(m => m.Name)
                .ToListAsync();
        }

        public async Task AddBrandWithModelsAsync(Brand brand, List<string> modelNames)
        {
            brand.Models = modelNames.Select(name => new Model { Name = name, Brand = brand }).ToList();
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> BrandExistsAsync(string brandName)
        {
            return await _context.Brands.AnyAsync(b => b.Name == brandName);
        }

        public async Task<Brand> GetBrandByNameAsync(string brandName)
        {
            return await _context.Brands.Include(b => b.Models).FirstOrDefaultAsync(b => b.Name == brandName);
        }

        public async Task AddModelsToExistingBrandAsync(Brand brand, List<string> modelNames)
        {
            var models = modelNames.Select(name => new Model { Name = name, Brand = brand }).ToList();
            _context.Models.AddRange(models);
            await _context.SaveChangesAsync();
        }
    }
}
