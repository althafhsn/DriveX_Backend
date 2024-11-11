using DriveX_Backend.Entities.Cars;
using DriveX_Backend.IRepository;
using DriveX_Backend.IServices;
using DriveX_Backend.Repository;

namespace DriveX_Backend.Services
{
    public class BrandService:IBrandService
    {
        private readonly IBrandRepository _repository;
        public BrandService(IBrandRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<string>> GetOrAddModelsByBrandNameAsync(string brandName, List<string> modelNames)
        {
            // Check if the brand exists
            var brand = await _repository.GetBrandByNameAsync(brandName);

            if (brand == null)
            {
                // Brand doesn't exist; create it with the provided models
                var newBrand = new Brand { Name = brandName };
                await _repository.AddBrandWithModelsAsync(newBrand, modelNames);
                return modelNames;
            }
            else if (brand.Models == null || !brand.Models.Any())
            {
                // Brand exists but has no models; add the provided models
                await _repository.AddModelsToExistingBrandAsync(brand, modelNames);
                return modelNames;
            }
            else
            {
                // Brand exists and has models; return them
                return brand.Models.Select(m => m.Name).ToList();
            }
        }
    }
}
