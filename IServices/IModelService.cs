using DriveX_Backend.Entities.Cars.Models;

namespace DriveX_Backend.IServices
{
    public interface IModelService
    {
        Task<IEnumerable<ModelResponseDto>> GetModelsByBrandAsync(Guid brandId);
        Task<ModelResponseDto> AddModelAsync(ModelRequestDTO modelRequestDto);

    }
}
