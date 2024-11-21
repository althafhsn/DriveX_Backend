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

        public async Task<List<CarDTO>> GetAllCarsAsync()
        {
            var cars = await _carRepository.GetAllCarsAsync();

            return cars.Select(car => new CarDTO
            {
                Id = car.Id,
                BrandId = car.Brand.Id,
                ModelId = car.Model.Id,
                RegNo = car.RegNo,
                PricePerDay = car.PricePerDay,
                GearType = car.GearType,
                FuelType = car.FuelType,
                Mileage = car.Mileage,
                SeatCount = car.SeatCount,
                Images = car.Images.Select(img => new ImageDTO
                {
                    Id = img.Id,
                    ImagePath = img.ImagePath
                }).ToList()
            }).ToList();
        }

        public async Task<List<CarSummaryDTO>> GetAllCars()
        {
            var cars = await _carRepository.GetAllCars();

            // Transform entities to DTOs
            var carSummaryDtos = cars.Select(car => new CarSummaryDTO
            {
                Id = car.Id,
                RegNo = car.RegNo,
                Status = car.Status,
                Image = car.Images.FirstOrDefault()?.ImagePath // Only the first image
            }).ToList();

            return carSummaryDtos;
        }

        public async Task<CarDTO> UpdateCarAsync(Guid id, UpdateCarDTO updateCarDto)
        {
            var car = await _carRepository.GetCarByIdAsync(id);
            if (car == null)
            {
                return null;
            }

            // Update car properties
            if (updateCarDto.PricePerDay != 0) car.PricePerDay = updateCarDto.PricePerDay;
            if (!string.IsNullOrEmpty(updateCarDto.GearType)) car.GearType = updateCarDto.GearType;
            if (!string.IsNullOrEmpty(updateCarDto.FuelType)) car.FuelType = updateCarDto.FuelType;
            if (!string.IsNullOrEmpty(updateCarDto.Mileage)) car.Mileage = updateCarDto.Mileage;
            if (!string.IsNullOrEmpty(updateCarDto.SeatCount)) car.SeatCount = updateCarDto.SeatCount;

            // Handle Images Update
            if (updateCarDto.Images != null && updateCarDto.Images.Any())
            {
                // Load existing images from the database
                var existingImages = car.Images.ToList();

                // Map new images from the request
                var newImages = updateCarDto.Images.Select(dto => new CarImage
                {
                    Id = dto.Id != Guid.Empty ? dto.Id : Guid.NewGuid(),
                    ImagePath = dto.ImagePath,
                    CarId = car.Id
                }).ToList();

                // Retain existing images that were not included in the update request
                var retainedImages = existingImages
                    .Where(existing => newImages.All(newImg => newImg.Id != existing.Id))
                    .ToList();

                // Combine retained and new images, ensuring no duplicates and limiting to 4
                car.Images = retainedImages.Concat(newImages).DistinctBy(img => img.ImagePath).Take(4).ToList();
            }

            // Save changes
            await _carRepository.UpdateAsync(car);

            // Return updated DTO
            return new CarDTO
            {
                Id = car.Id,
                BrandId = car.BrandId,
                ModelId = car.ModelId,
                RegNo = car.RegNo,
                PricePerDay = car.PricePerDay,
                GearType = car.GearType,
                FuelType = car.FuelType,
                Mileage = car.Mileage,
                SeatCount = car.SeatCount,
                Images = car.Images.Select(image => new ImageDTO
                {
                    Id = image.Id,
                    ImagePath = image.ImagePath
                }).ToList()
            };
        }

        public async Task<bool> DeleteCarAsync(Guid id)
        {
            var car = await _carRepository.GetCarByIdAsync(id);
            if (car == null)
            {
                return false; // Car not found
            }

            await _carRepository.DeleteAsync(car);
            return true;
        }
    }
}
