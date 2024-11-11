using DriveX_Backend.Entities.Cars.Models;

namespace DriveX_Backend.IServices
{
    public interface IBrandService
    {
        Task<IEnumerable<BrandDTO>> GetAllBrandsAsync();
        Task<BrandDTO> AddBrandAsync(BrandRequestDTO brandRequestDTO);
    }
}
