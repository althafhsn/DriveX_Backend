using DriveX_Backend.Entities.Cars.Models;
using DriveX_Backend.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DriveX_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelController : ControllerBase
    {
        private readonly IModelService _modelService;

        public ModelController(IModelService modelService)
        {
            _modelService = modelService;
        }


        [HttpGet("brand/{brandId}")]
        public async Task<IActionResult> GetModelsByBrand(Guid brandId)
        {
            var models = await _modelService.GetModelsByBrandAsync(brandId);
            return Ok(models);
        }


        [HttpPost("add-new-model")]
        public async Task<IActionResult> AddModel(ModelRequestDTO modelDto)
        {
            try
            {
               var data= await _modelService.AddModelAsync(modelDto);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
