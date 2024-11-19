using DriveX_Backend.Entities.Cars;
using DriveX_Backend.Entities.Cars.Models;

namespace DriveX_Backend.IRepository
{
    public interface IModelRepository
    {
        Task<IEnumerable<Model>> GetByBrandIdAsync(Guid brandId);
        Task<Model> GetByIdAsync(Guid modelId);
        Task<Model> GetByNameAndBrandIdAsync(Guid brandId, string modelName);
        Task<Model> AddModelAsync(Model model);
        Task<bool> ExistsAsync(Guid brandId, string modelName);
    }
}
