using DriveX_Backend.Entities.Cars;

namespace DriveX_Backend.IRepository
{
    public interface IBrandRepository
    {
        Task<List<string>> GetModelNamesByBrandNameAsync(string brandName);
        Task AddBrandWithModelsAsync(Brand brand, List<string> modelNames);
        Task<bool> BrandExistsAsync(string brandName);
        Task<Brand> GetBrandByNameAsync(string brandName);
        Task AddModelsToExistingBrandAsync(Brand brand, List<string> modelNames);
    }
}
