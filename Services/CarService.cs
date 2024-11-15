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
            // Validate the BrandId
            var brand = await _brandRepository.GetByIdAsync(carRequestDto.BrandId);
            if (brand == null)
            {
                throw new Exception("Brand not found");
            }

            // Validate the ModelId
            var model = await _modelRepository.GetByIdAsync(carRequestDto.ModelId);
            if (model == null || model.BrandId != carRequestDto.BrandId)
            {
                throw new Exception("Model not found or does not belong to the specified brand");
            }

            // Create a new Car entity from the request DTO
            var car = new Car
            {
                BrandId = carRequestDto.BrandId,
                ModelId = carRequestDto.ModelId,
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
                    ImagePath = i.ImagePath,
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
                Id = Guid.NewGuid(),
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
                    Id = Guid.NewGuid(),
                    ImagePath = i.ImagePath
                }).ToList()
            };

            return carDto;
        }


        public async Task<CarDTO> GetCarByIdAsync(Guid id)
        {
            // Retrieve the car entity with images from the repository
            var car = await _carRepository.GetCarByIdAsync(id);

            // If car is null, return null
            if (car == null)
            {
                return null;
            }

            // Map the Car entity to CarDTO and limit images to 4
            return new CarDTO
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
                Images = car.Images?.Take(4).Select(i => new ImageDTO
                {
                    Id = i.Id,
                    ImagePath = i.ImagePath
                }).ToList()
            };
        }

    }
}
