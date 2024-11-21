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

        [HttpGet("GetCarById{id}")]
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

        [HttpGet("GetAllCars")]
        public async Task<IActionResult> GetAllCarsAsync()
        {
            try
            {
                var cars = await _carService.GetAllCarsAsync();
                return Ok(cars);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllCars()
        {
            var cars = await _carService.GetAllCars();
            return Ok(cars);
        }

        [HttpPut("UpdateCar{id}")]
        public async Task<IActionResult> UpdateCar(Guid id, [FromBody] UpdateCarDTO updateCarDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedCar = await _carService.UpdateCarAsync(id, updateCarDto);
            if (updatedCar == null)
            {
                return NotFound(new { message = "Car not found." });
            }

            return Ok(updatedCar);
        }

        [HttpDelete("DeleteCar{id}")]
        public async Task<IActionResult> DeleteCar(Guid id)
        {
            bool isDeleted = await _carService.DeleteCarAsync(id);
            if (!isDeleted)
            {
                return NotFound(new { Message = "Car not found." });
            }

            return Ok(new { Message = "Car successfully deleted." });
        }

    }
}
