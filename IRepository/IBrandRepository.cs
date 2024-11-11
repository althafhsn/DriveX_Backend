using DriveX_Backend.Entities.Cars;

namespace DriveX_Backend.IRepository
{
    public interface IBrandRepository
    {
        Task<IEnumerable<Brand>> GetAllBrandAsync();
        Task<Brand> GetByIdAsync(Guid id);
        Task<Brand> AddBrandAsync(Brand brand);
        Task<bool> ExistsAsync(string brandName);

        Task<bool> ExistsBrandId(Guid brandId);

    }
}
