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
        private readonly ICarRepository _carRepository;
        public CarController(ICarService carService,ICarRepository carRepository)
        {
            _carService = carService;
            _carRepository = carRepository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCarById(Guid id)
        {
            // Fetch the car by ID from the repository
            var car = await _carRepository.GetCarByIdAsync(id);

            // If car not found, return a 404 (Not Found)
            if (car == null)
            {
                return NotFound(new { message = "Car not found" });
            }

            // Assuming you have a DTO for Car and CarImage
            var carDto = new CarDTO
            {
                Id = car.Id,
                BrandId = car.BrandId,
                ModelId = car.ModelId,
                RegNo = car.RegNo,
                PricePerDay = car.PricePerDay,
                PricePerHour = car.PricePerHour,
                GearType = car.GearType,
                FuelType = car.FuelType,
                Mileage = car.Mileage,
                SeatCount = car.SeatCount,
                // Mapping the images to CarImageDTO to return relevant information
                Images = car.Images?.Take(4).Select(i => new ImageDTO
                {
                    Id = i.Id,
                    ImagePath = i.ImagePath
                }).ToList() // Limiting to 4 images
            };

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
