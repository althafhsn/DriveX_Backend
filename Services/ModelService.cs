using DriveX_Backend.Entities.Cars;
using DriveX_Backend.Entities.Cars.Models;
using DriveX_Backend.IRepository;
using DriveX_Backend.IServices;

namespace DriveX_Backend.Services
{
    public class ModelService : IModelService
    {
        private readonly IModelRepository _modelRepository;
        private readonly IBrandRepository _brandRepository;

        public ModelService(IModelRepository modelRepository, IBrandRepository brandRepository)
        {
            _modelRepository = modelRepository;
            _brandRepository = brandRepository;
        }

        public async Task<IEnumerable<ModelResponseDto>> GetModelsByBrandAsync(Guid brandId)
        {
            var models = await _modelRepository.GetByBrandIdAsync(brandId);
            return models.Select(m => new ModelResponseDto
            {
                Id = m.Id,
                Name = m.Name,
                BrandId = m.BrandId,
            });
        }

        public async Task<ModelResponseDto> AddModelAsync(ModelRequestDTO modelRequestDto)
        {

            if (!await _brandRepository.ExistsBrandId(modelRequestDto.BrandId))
                throw new Exception("Brand must be selected to add a model.");

            if (await _modelRepository.ExistsAsync(modelRequestDto.BrandId, modelRequestDto.Name))
                throw new Exception("Model with this name already exists for the selected brand.");

            var model = new Model
            {
                Name = modelRequestDto.Name,
                BrandId = modelRequestDto.BrandId
            };
           var data =  await _modelRepository.AddModelAsync(model);

            var res = new ModelResponseDto
            {
                Id = data.Id,
                Name = data.Name,
                BrandId = data.BrandId
            };
            return res;
        }

    }
}
