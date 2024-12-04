using DriveX_Backend.Entities.RentalRequest.Models;
using DriveX_Backend.Entities.RentalRequest;
using DriveX_Backend.IRepository;
using DriveX_Backend.IServices;
using DriveX_Backend.Repository;
using DriveX_Backend.Entities.Cars.Models;

using Microsoft.EntityFrameworkCore;

using DriveX_Backend.Entities.Users.UserDTO;
using DriveX_Backend.Entities.Cars;
using DriveX_Backend.Entities.Users;
using DriveX_Backend.Utility;

using DriveX_Backend.Migrations;



namespace DriveX_Backend.Services
{
    public class RentalRequestService:IRentalRequestService
    {
        private readonly IRentalRequestRepository _repository;
        private readonly ICarRepository _carRepository;
        private readonly IUserRepository _userRepository;
        private readonly WhatsAppService _whatsAppService;
        public RentalRequestService(IRentalRequestRepository repository, ICarRepository carRepository, WhatsAppService whatsAppService, IUserRepository userRepository)
        {
            _repository = repository;
            _carRepository = carRepository;
            _userRepository = userRepository;
            _whatsAppService = whatsAppService;
        }

        public async Task<AddRentalResponseDTO> AddRentalRequestAsync(AddRentalRequestDTO requestDTO)
        {
            var car = await _carRepository.GetCarByIdAsync(requestDTO.CarId);
            if (car == null)
            {
                throw new Exception("Car not found.");
            }
            var existingRental = await _repository.GetRentalRequestByCarIdAsync(requestDTO.CarId);

            if (existingRental != null && existingRental.Status == "rented")
            {
                var bufferStartDate = existingRental.StartDate.AddDays(-2);
                var bufferEndDate = existingRental.EndDate.AddDays(2);

                if (!(requestDTO.EndDate < bufferStartDate || requestDTO.StartDate > bufferEndDate))
                {
                    throw new Exception("The requested rental period conflicts with an existing rental.");
                }
            }

            int duration = (int)(requestDTO.EndDate - requestDTO.StartDate).TotalDays;
            if (duration <= 0)
            {
                throw new Exception("End date must be later than start date.");
            }

            decimal totalPrice = car.PricePerDay * duration;

            string action = "Pending";
            string status = "Request";

            var rentalRequest = new RentalRequest
            {
                Id = Guid.NewGuid(),
                CarId = requestDTO.CarId,
                UserId = requestDTO.UserId,
                StartDate = requestDTO.StartDate,
                EndDate = requestDTO.EndDate,
                Duration = duration,
                TotalPrice = totalPrice,
                RequestDate = DateTime.Today,
                Action = action,
                Status = status
            };

            await _repository.AddRentalRequestAsync(rentalRequest);

            return new AddRentalResponseDTO
            {
                Id = rentalRequest.Id,
                CarId = rentalRequest.CarId,
                UserId = rentalRequest.UserId,
                RequestDate = rentalRequest.RequestDate,
                Duration = rentalRequest.Duration,
                TotalPrice = rentalRequest.TotalPrice,
                StartDate = rentalRequest.StartDate.ToString("yyyy-MM-dd"),
                EndDate = rentalRequest.EndDate.ToString("yyyy-MM-dd"),
                Action = rentalRequest.Action,
                Status = rentalRequest.Status
            };
        }

        public async Task UpdateRentalActionAsync(Guid id, string action)
        {
            var rentalRequest = await _repository.GetByIdAsync(id);
            if (rentalRequest == null)
            {
                throw new KeyNotFoundException($"Rental request with ID {id} not found.");
            }
            var existingRentalRequest = await _repository.GetRentalRequestByCarIdAsync(rentalRequest.CarId);
            if (existingRentalRequest != null && existingRentalRequest.Id != rentalRequest.Id)
            {
                var rentCar = await _carRepository.GetCarByIdAsync(rentalRequest.CarId);
                if (rentCar == null)
                {
                    throw new KeyNotFoundException($"Car with ID {rentalRequest.CarId} not found.");
                }

                if (rentCar.Action.Equals("confirmed", StringComparison.OrdinalIgnoreCase))
                {
                    var bufferStartDate = rentalRequest.StartDate.AddDays(-2);
                    var bufferEndDate = rentalRequest.EndDate.AddDays(2);

                    if ((rentalRequest.EndDate < bufferStartDate || rentalRequest.StartDate > bufferEndDate))
                    {
                        throw new Exception("The requested rental period conflicts with an existing rental.");
                    }
                }
            }
                rentalRequest.Action = action;

            await _repository.UpdateAsync(rentalRequest);

            if (action.Equals("Approved", StringComparison.OrdinalIgnoreCase))
            {
                rentalRequest.Status = "rented";
                var car = await _carRepository.GetCarByIdAsync(rentalRequest.CarId);
                if (car == null)
                {
                    throw new KeyNotFoundException($"Car with ID {rentalRequest.CarId} not found.");
                }

                car.OngoingRevenue += rentalRequest.TotalPrice;

                var user = await _userRepository.GetCustomerByIdAsync(rentalRequest.UserId);
                if (user == null)
                {
                    throw new KeyNotFoundException($"User with ID {rentalRequest.UserId} not found.");
                }

                user.OngoingRevenue += rentalRequest.TotalPrice;

                await _userRepository.UpdateCustomerAsync(user);

                // Save the updated car
                await _carRepository.UpdateAsync(car);
            }
        }

        public async Task UpdateRentalStatusAsync(Guid id, string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                throw new ArgumentException("Status cannot be empty.");
            }

            var rentalRequest = await _repository.GetByIdAsync(id);

            if (rentalRequest == null)
            {
                throw new KeyNotFoundException("Rental request not found.");
            }

            rentalRequest.Status = status;
            var car = await _carRepository.GetCarByIdAsync(rentalRequest.CarId);

            if (status == "returned" && rentalRequest.Car != null)
            {

                car.TotalRevenue += car.OngoingRevenue;
               car.OngoingRevenue = 0;

                await _carRepository.UpdateAsync(car);
            }

            var user = await _userRepository.GetCustomerByIdAsync(rentalRequest.UserId);
            if( status == "Returned" && rentalRequest.User != null)
            {
                user.TotalRevenue += user.OngoingRevenue;
                user.OngoingRevenue = 0;
            }
           
            await _repository.UpdateRentalRequestAsync(rentalRequest);
        }

        public async Task<List<OngoingRentalsDTO>> GetAllOngoingRentals()
        {
            var rentalRequest = await _repository.GetAllOngoingRentals();
            var carIds = rentalRequest.Select(r => r.CarId).Distinct().ToList();
            var userIds = rentalRequest.Select(r => r.UserId).Distinct().ToList();

            var cars = new List<Car>();
            foreach (var carId in carIds)
            {
                var car = await _carRepository.GetCarByIdAsync(carId);
                if (car != null) cars.Add(car);
            }

            var users = new List<User>();
            foreach (var userId in userIds)
            {
                var user = await _userRepository.GetCustomerByIdAsync(userId);
                if (user != null) users.Add(user);
            }

            var result = rentalRequest.Select(r => new OngoingRentalsDTO
            {
                Id = r.Id,
                CarId = r.CarId,
                UserId = r.UserId,
                RequestDate = DateTime.Now,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                TotalPrice = r.TotalPrice,
                Status = r.Status,
                RegNo = cars.FirstOrDefault(c => c.Id == r.CarId)?.RegNo ?? "N/A",
                NIC = users.FirstOrDefault(u => u.Id == r.UserId)?.NIC ?? "N/A"
            }).ToList();
            return result;

        }

        public async Task<List<OngoingRentalsDTO>> GetAllRented()
        {
            var rentalRequest = await _repository.GetAllRenteds();
            var carIds = rentalRequest.Select(r => r.CarId).Distinct().ToList();
            var userIds = rentalRequest.Select(r => r.UserId).Distinct().ToList();

            var cars = new List<Car>();
            foreach (var carId in carIds)
            {
                var car = await _carRepository.GetCarByIdAsync(carId);
                if (car != null) cars.Add(car);
            }

            var users = new List<User>();
            foreach (var userId in userIds)
            {
                var user = await _userRepository.GetCustomerByIdAsync(userId);
                if (user != null) users.Add(user);
            }

            var result = rentalRequest.Select(r => new OngoingRentalsDTO
            {
                Id = r.Id,
                CarId = r.CarId,
                UserId = r.UserId,
                RequestDate = DateTime.Now,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                TotalPrice = r.TotalPrice,
                Status = r.Status,
                RegNo = cars.FirstOrDefault(c => c.Id == r.CarId)?.RegNo ?? "N/A",
                NIC = users.FirstOrDefault(u => u.Id == r.UserId)?.NIC ?? "N/A"
            }).ToList();
            return result;

        }

        public async Task<List<recentRentalRequestDTO>> GetRecentRentalRequests()
        {
            var rentalRequest = await _repository.GetRecentRentalRequest();
            var carIds = rentalRequest.Select(r => r.CarId).Distinct().ToList();
            var userIds = rentalRequest.Select(r => r.UserId).Distinct().ToList();

            var cars = new List<Car>();
            foreach (var carId in carIds)
            {
                var car = await _carRepository.GetCarByIdAsync(carId);
                if (car != null) cars.Add(car);
            }

            var users = new List<User>();
            foreach (var userId in userIds)
            {
                var user = await _userRepository.GetCustomerByIdAsync(userId);
                if (user != null) users.Add(user);
            }

            var result = rentalRequest.Select(r => new recentRentalRequestDTO
            {
                Id = r.UserId,
                CarId = r.CarId,
                UserId = r.UserId,
                Status = r.Status,
                TotalPrice = r.TotalPrice,
                RegNo = cars.FirstOrDefault(c => c.Id == r.CarId)?.RegNo ?? "N/A",
                FirstName = users.FirstOrDefault(u => u.Id == r.UserId)?.FirstName ?? "N/A",
                LastName = users.FirstOrDefault(u => u.Id == r.UserId)?.LastName ?? "N/A",

            }).ToList();
            return result;
        }

        public async Task<List<GetAllRentalDTO>> GetAllRentalRequestsAsync()
        {
            var rentalRequests = await _repository.GetAllRentalRequestsAsync();


            // Mapping entities to DTOs
            var rentalDTOs = rentalRequests.Select(r => new GetAllRentalDTO
            {
                Id = r.Id,
                RequestDate = r.RequestDate,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                Duration = r.Duration,
                TotalPrice = r.TotalPrice,
                Action = r.Action,
                Status = r.Status,
                UserId = r.User.Id,
                FirstName = r.User.FirstName,
                LastName = r.User.LastName,
                NIC = r.User.NIC,
                Licence = r.User.Licence,
                Email = r.User.Email,
                Addresses = r.User.Addresses.Select(a => new UserAddressDTO
                {
                    Id = a.Id,
                    HouseNo = a.HouseNo,
                    Street1 = a.Street1,
                    Street2 = a.Street2,
                    City = a.City,
                    ZipCode = a.ZipCode,
                    Country = a.Country
                }).ToList(),
                PhoneNumbers = r.User.PhoneNumbers.Select(p => new UserPhoneNumberDTO
                {
                    Id = p.Id,
                    Mobile1 = p.Mobile1
                }).ToList(),
                CarId = r.Car.Id,
                BrandId = r.Car.Brand.Id,
                BrandName = r.Car.Brand.Name,
                ModelId = r.Car.Model.Id,
                ModelName = r.Car.Model.Name,
                RegNo = r.Car.RegNo,
                PricePerDay = r.Car.PricePerDay,
                GearType = r.Car.GearType,
                FuelType = r.Car.FuelType,
                Mileage = r.Car.Mileage,
                SeatCount = r.Car.SeatCount,
                carStatus = r.Car.Status,
                OngoingRevenue = r.Car.OngoingRevenue,
                TotalRevenue = r.Car.TotalRevenue,
                Images = r.Car.Images.Select(i => new ImageDTO
                {
                    Id = i.Id,
                    ImagePath = i.ImagePath
                }).ToList()
            }).ToList();



            return rentalDTOs;
        }
        public async Task<List<getCustomerRentalDTO>> GetRentalRequestsByCustomerIdAsync(Guid id)
        {
            var rentalRequests = await _repository.GetRentalRequestsByCustomerIdAsync(id);

            var result = rentalRequests.Select(r => new getCustomerRentalDTO
            {
                Id = r.Id,
                CarId = r.CarId,
                UserId = r.UserId,
                RegNo = r.Car?.RegNo ?? "N/A",  
                BrandName = r.Car?.Brand?.Name ?? "N/A", 
                ModelName = r.Car?.Model?.Name ?? "N/A", 
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                Action = r.Action,
                Status = r.Status
            }).ToList();

            return result;
        }


    }
}
