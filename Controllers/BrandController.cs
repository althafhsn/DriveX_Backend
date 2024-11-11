using DriveX_Backend.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DriveX_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;
        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet("{name}/models")]
        public async Task<IActionResult> GetOrAddModelsByBrandName(string name, [FromQuery] List<string> modelNames)
        {
            // This will add the brand and models if necessary or return existing models
            var models = await _brandService.GetOrAddModelsByBrandNameAsync(name, modelNames);

            if (models == null || !models.Any())
            {
                return NotFound();
            }
            return Ok(models);
        }
    }
}
