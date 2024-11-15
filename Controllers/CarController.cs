using DriveX_Backend.Entities.Cars.Models;
using DriveX_Backend.IRepository;
using DriveX_Backend.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DriveX_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly ICarService _carService;
        public CarController(ICarService carService)
        {
            _carService = carService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCarById(Guid id)
        {
            // Fetch the car DTO from the service
            var carDto = await _carService.GetCarByIdAsync(id);

            // If car not found, return a 404 (Not Found)
            if (carDto == null)
            {
                return NotFound(new { message = "Car not found" });
            }

            // Return the car DTO with a 200 (OK) status code
            return Ok(carDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddCar(CarRequestDTO carRequestDto)
        {
            var result = await _carService.AddCarAsync(carRequestDto);
            return CreatedAtAction(nameof(GetCarById), new { id = result.Id }, result);
        }


    }
}
