using DriveX_Backend.Entities.Cars.Models;
using DriveX_Backend.Entities.Cars;
using DriveX_Backend.IRepository;
using DriveX_Backend.IServices;
using DriveX_Backend.Repository;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using DriveX_Backend.Entities.Users.UserDTO;
using DriveX_Backend.Entities.Users;
using DriveX_Backend.Entities.RentalRequest;

namespace DriveX_Backend.Services
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly IModelRepository _modelRepository;
        private readonly IRentalRequestRepository _rentalRequestRepository;
        private readonly IUserRepository _userRepository;
        public CarService(ICarRepository carRepository, IBrandRepository brandRepository, IModelRepository modelRepository, IRentalRequestRepository rentalRequestRepository, IUserRepository userRepository)
        {
            _carRepository = carRepository;
            _brandRepository = brandRepository;
            _modelRepository = modelRepository;
            _rentalRequestRepository = rentalRequestRepository;
            _userRepository = userRepository;
        }

        public async Task<CarDTO> AddCarAsync(CarRequestDTO carRequestDto)
        {
            // Validate Brand
            var brand = await _brandRepository.GetByIdAsync(carRequestDto.BrandId);
            if (brand == null)
            {
                throw new Exception("Brand not found");
            }

            // Validate Model
            var model = await _modelRepository.GetByIdAsync(carRequestDto.ModelId);
            if (model == null || model.BrandId != carRequestDto.BrandId)
            {
                throw new ValidationException("Model not found or does not belong to the specified brand");
            }

            var existingCar = await _carRepository.GetByRegNoAsync(carRequestDto.RegNo);
            if (existingCar != null)
            {
                throw new ValidationException("A car with this registration number already exists");
            }

            // Map DTO to Entity
            var car = new Car
            {
                BrandId = carRequestDto.BrandId,
                ModelId = carRequestDto.ModelId,
                RegNo = carRequestDto.RegNo,
                Year = carRequestDto.Year,
                PricePerDay = carRequestDto.PricePerDay,
                GearType = carRequestDto.GearType,
                FuelType = carRequestDto.FuelType,
                Mileage = carRequestDto.Mileage,
                SeatCount = carRequestDto.SeatCount,
                Status = "Available",
                Images = carRequestDto.Images.Select(i => new CarImage
                {
                    ImagePath = i.ImagePath
                }).Take(4).ToList()
            };

            // Save to Database

            var addedCar = await _carRepository.AddCarAsync(car);

            foreach (var image in addedCar.Images)
            {
                image.CarId = addedCar.Id;
            }

            await _carRepository.SaveImagesAsync(addedCar.Images);


            // Map to DTO
            var carDto = new CarDTO
            {
                Id = addedCar.Id,
                BrandId = carRequestDto.BrandId,
                BrandName = brand.Name,
                ModelId = carRequestDto.ModelId,
                ModelName = model.Name,
                RegNo = addedCar.RegNo,
                Year = addedCar.Year,
                PricePerDay = addedCar.PricePerDay,
                GearType = addedCar.GearType,
                FuelType = addedCar.FuelType,
                Mileage = addedCar.Mileage,
                SeatCount = addedCar.SeatCount,
                Status = addedCar.Status,
                Images = addedCar.Images.Select(i => new ImageDTO
                {
                    Id = i.Id,
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
                BrandName = car.Brand.Name,
                ModelId = car.ModelId,
                ModelName = car.Model.Name,
                RegNo = car.RegNo,
                Year = car.Year,
                PricePerDay = car.PricePerDay,
                GearType = car.GearType,
                FuelType = car.FuelType,
                Mileage = car.Mileage,
                SeatCount = car.SeatCount,
                Status = car.Status,
                Images = car.Images?.Take(4).Select(i => new ImageDTO
                {
                    Id = i.Id,
                    ImagePath = i.ImagePath
                }).ToList()
            };
        }

        public async Task<CarCustomerDTO>GetCarById(Guid id)
        {
            var car = await _carRepository.GetCarByIdAsync(id);
            if (car == null)
            {
                return null;
            }
            var rental = await _rentalRequestRepository.GetRentalRequestByCarIdAsync(id);
            return new CarCustomerDTO
            {
                Id = car.Id,
                BrandId = car.BrandId,
                BrandName = car.Brand.Name,
                ModelId = car.ModelId,
                ModelName = car.Model.Name,
                RegNo = car.RegNo,
                PricePerDay = car.PricePerDay,
                Year = car.Year,
                GearType = car.GearType,
                FuelType = car.FuelType,
                Mileage = car.Mileage,
                SeatCount = car.SeatCount,
                Status = car.Status,
                Images = car.Images?.Take(4).Select(i => new ImageDTO
                {
                    Id = i.Id,
                    ImagePath = i.ImagePath
                }).ToList(),
                StartDate = rental?.StartDate,
                EndDate = rental?.EndDate,
                Duration = rental?.Duration ?? 0,
                TotalPrice = rental?.TotalPrice ?? 0
            };
        }

        public async Task<List<CarDTO>> GetAllCarsAsync()
        {
            var cars = await _carRepository.GetAllCarsAsync();
            var rentalRequests = await _rentalRequestRepository.GetAllRentalRequestsAsync();
            var currentDate = DateTime.UtcNow.Date;

            return cars.Select(car =>
            {
                var rentalRequest = rentalRequests.FirstOrDefault(r =>
                    r.CarId == car.Id);
                var status = rentalRequest != null
                            ? (rentalRequest.StartDate.Date == currentDate.AddDays(1).Date ? "Unavailable" : "Available")
                            : "Available";

                return new CarDTO
                {
                    Id = car.Id,
                    BrandId = car.BrandId,
                    BrandName = car.Brand.Name,
                    ModelId = car.ModelId,
                    ModelName = car.Model.Name,
                    RegNo = car.RegNo,
                    Year = car.Year,
                    PricePerDay = car.PricePerDay,
                    GearType = car.GearType,
                    FuelType = car.FuelType,
                    Mileage = car.Mileage,
                    SeatCount = car.SeatCount,
                    Status = status,
                    Images = car.Images.Select(img => new ImageDTO
                    {
                        Id = img.Id,
                        ImagePath = img.ImagePath
                    }).ToList()
                };
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


            if (updateCarDto.Images != null && updateCarDto.Images.Any())
            {
                var existingImages = car.Images.ToList();

                var newImages = updateCarDto.Images.Select(dto => new CarImage
                {
                    Id = dto.Id != Guid.Empty ? dto.Id : Guid.NewGuid(),
                    ImagePath = dto.ImagePath,
                    CarId = car.Id
                }).ToList();

                var retainedImages = existingImages
                    .Where(existing => newImages.All(newImg => newImg.Id != existing.Id))
                    .ToList();

                car.Images = retainedImages.Concat(newImages).DistinctBy(img => img.ImagePath).Take(4).ToList();
            }

            await _carRepository.UpdateAsync(car);

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
                Year = car.Year,
                Images = car.Images.Select(image => new ImageDTO
                {
                    Id = image.Id,
                    ImagePath = image.ImagePath
                }).ToList()
            };
        }
        public async Task<(decimal TotalOngoingRevenue, decimal TotalRevenue, int TotalCars, int TotalCustomers)> GetTotalRevenuesAsync()
        {
            return await _carRepository.GetTotalRevenuesAsync();
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

        public async Task<(CarDTO car, List<UserDTO> customers, string message)> GetCarDetailsWithRentalInfoAsync(Guid carId)
        {
            var car = await _carRepository.GetCarByIdAsync(carId);
            if (car == null)
            {
                return (null, new List<UserDTO>(), "Car not found.");
            }

            var rentalRequest = await _rentalRequestRepository.GetRentalRequestByCarIdAsync(carId);

            if (rentalRequest == null)
            {
                var carDto = MapCarToDto(car);
                return (carDto, new List<UserDTO>(), "Car details returned. No associated rental request.");
            }

            if (rentalRequest.Action.Equals("Approved", StringComparison.OrdinalIgnoreCase))
            {
                var customer = await _userRepository.GetCustomerByIdAsync(rentalRequest.UserId);
                var carDto = MapCarToDto(car,rentalRequest);

                var customerDtoList = customer != null ? new List<UserDTO> { MapUserToDto(customer) } : new List<UserDTO>();
                return (carDto, customerDtoList, "Car and customer details returned.");
            }

            return (MapCarToDto(car), new List<UserDTO>(), "Car details returned. Rental request not approved.");
        }


        private CarDTO MapCarToDto(Car car, RentalRequest rentalRequest = null)
        {
            return new CarDTO
            {
                Id = car.Id,
                BrandId = car.BrandId,
                BrandName = car.Brand.Name,
                ModelId = car.ModelId,
                ModelName = car.Model.Name,
                RegNo = car.RegNo,
                Year = car.Year,
                PricePerDay = car.PricePerDay,
                FuelType = car.FuelType,
                GearType = car.GearType,
                SeatCount = car.SeatCount,
                Mileage =car.Mileage,
                Images = car.Images.Select(img => new ImageDTO { Id = img.Id, ImagePath = img.ImagePath }).ToList(),
                Status = car.Status,
                OngoingRevenue = car.OngoingRevenue,
                TotalRevenue = car.TotalRevenue,
                StartDate = rentalRequest?.StartDate,
                EndDate = rentalRequest?.EndDate,
                Duration = rentalRequest?.Duration,
                RentalRequestStatus = rentalRequest?.Status
            };
        }

        private UserDTO MapUserToDto(User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                NIC = user.NIC,
                PhoneNumbers = user.PhoneNumbers?.Select(p => new PhoneNumberResponseDTO
                {
                    Id=p.Id,
                    Mobile1 = p.Mobile1
                }).ToList()
            };
        }
    }
}
