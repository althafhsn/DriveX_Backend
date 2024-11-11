using DriveX_Backend.Entities.Cars.Models;
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

        [HttpGet("get-all-brand")]

        public async Task<IActionResult> GetAllBrands()
        {
            var brands = await _brandService.GetAllBrandsAsync();
            return Ok(brands);
        }

        [HttpPost("add-new-brand")]

        public async Task<IActionResult> AddNewBrand([FromBody] BrandRequestDTO brandDTO)
        {
            try
            {
               var data = await _brandService.AddBrandAsync(brandDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
