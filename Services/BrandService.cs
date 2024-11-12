using DriveX_Backend.Entities.Cars;
using DriveX_Backend.Entities.Cars.Models;
using DriveX_Backend.IRepository;
using DriveX_Backend.IServices;
using DriveX_Backend.Repository;

namespace DriveX_Backend.Services
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        public BrandService(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<IEnumerable<BrandDTO>> GetAllBrandsAsync()
        {
            var brands = await _brandRepository.GetAllBrandAsync();
            return brands.Select(b => new BrandDTO
            {
                Id = b.Id,
                Name = b.Name
            });
        }

        public async Task<BrandDTO> AddBrandAsync(BrandRequestDTO brandRequestDTO)
        {
            if (await _brandRepository.ExistsAsync(brandRequestDTO.Name))
                throw new Exception("Brand with this name already exists.");

            var brand = new Brand
            {
                Name = brandRequestDTO.Name,
            };

            var brandResponse = await _brandRepository.AddBrandAsync(brand);

            var res = new BrandDTO
            {
                Id = brandResponse.Id,
                Name = brandResponse.Name,
            };
            return res;
            
        }



    }
}
