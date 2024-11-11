using DriveX_Backend.DB;
using DriveX_Backend.Entities.Cars;
using DriveX_Backend.IRepository;
using Microsoft.EntityFrameworkCore;

namespace DriveX_Backend.Repository
{
    public class BrandRepository : IBrandRepository
    {
        private readonly AppDbContext _context;
        public BrandRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Brand>> GetAllBrandAsync() => await _context.Brands.ToListAsync();
        public async Task<Brand> GetByIdAsync(Guid id) => await _context.Brands.FindAsync(id);

        public async Task<Brand> AddBrandAsync(Brand brand)
        {
            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();
            return brand;
        }

        public async Task<bool> ExistsAsync(string brandName)=>      
            await _context.Brands.AnyAsync(b=>b.Name.ToLower()== brandName.ToLower());

        public async Task<bool> ExistsBrandId(Guid brandId)=>
            await _context.Brands.AnyAsync(b=>b.Id==brandId);
        
    }
}
