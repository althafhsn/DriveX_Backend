using DriveX_Backend.Entities.Cars.Models;
using DriveX_Backend.Entities.Cars;
using DriveX_Backend.IRepository;
using DriveX_Backend.IServices;
using DriveX_Backend.Repository;

namespace DriveX_Backend.Services
{
    public class CarService:ICarService
    {
        private readonly ICarRepository _carRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly IModelRepository _modelRepository;
        public CarService(ICarRepository carRepository, IBrandRepository brandRepository, IModelRepository modelRepository)
        {
            _carRepository = carRepository;
            _brandRepository = brandRepository;
            _modelRepository = modelRepository;
        }

        public async Task<CarDTO> AddCarAsync(CarRequestDTO carRequestDto)
        {
            // Find or add the brand
            var brand = await _brandRepository.GetByNameAsync(carRequestDto.BrandName);
            if (brand == null)
            {
                brand = new Brand { Name = carRequestDto.BrandName };
                brand = await _brandRepository.AddBrandAsync(brand);
            }

            // Find or add the model
            var model = await _modelRepository.GetByNameAndBrandIdAsync(brand.Id, carRequestDto.ModelName);
            if (model == null)
            {
                model = new Model { Name = carRequestDto.ModelName, BrandId = brand.Id };
                model = await _modelRepository.AddModelAsync(model);
            }

            // Create a new Car entity from the request DTO
            var car = new Car
            {
                BrandId = brand.Id,
                ModelId = model.Id,
                RegNo = carRequestDto.RegNo,
                PricePerDay = carRequestDto.PricePerDay,
                PricePerHour = carRequestDto.PricePerHour,
                GearType = carRequestDto.GearType,
                FuelType = carRequestDto.FuelType,
                Mileage = carRequestDto.Mileage,
                SeatCount = carRequestDto.SeatCount,
                Status = "Available",
                // Map Images from DTO to CarImage entities
                Images = carRequestDto.Images.Select(i => new CarImage
                {
                    Id = i.Id,
                    ImagePath = i.ImagePath,
                    CarId = Guid.NewGuid() // CarId will be updated after the car is saved
                }).Take(4).ToList()
            };

            // Save the car to the database
            var addedCar = await _carRepository.AddCarAsync(car);

            // After car is saved, update the Image CarId with the added car's Id
            foreach (var image in addedCar.Images)
            {
                image.CarId = addedCar.Id;
            }

            // Save images separately
            await _carRepository.SaveImagesAsync(addedCar.Images);

            // Map the added Car entity to CarDTO to return in the response
            var carDto = new CarDTO
            {
                Id = addedCar.Id,
                BrandId = addedCar.BrandId,
                ModelId = addedCar.ModelId,
                RegNo = addedCar.RegNo,
                PricePerDay = addedCar.PricePerDay,
                PricePerHour = addedCar.PricePerHour,
                GearType = addedCar.GearType,
                FuelType = addedCar.FuelType,
                Mileage = addedCar.Mileage,
                SeatCount = addedCar.SeatCount,
                Images = addedCar.Images.Select(i => new ImageDTO
                {
                    Id = i.Id,
                    ImagePath = i.ImagePath
                }).ToList()
            };

            return carDto;
        }

    }
}
