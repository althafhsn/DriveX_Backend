using DriveX_Backend.Entities.RentalRequest.Models;
using DriveX_Backend.Entities.RentalRequest;
using DriveX_Backend.IRepository;
using DriveX_Backend.IServices;
using DriveX_Backend.Repository;
using DriveX_Backend.Entities.Cars.Models;
using Microsoft.EntityFrameworkCore;

namespace DriveX_Backend.Services
{
    public class RentalRequestService:IRentalRequestService
    {
        private readonly IRentalRequestRepository _repository;
        private readonly ICarRepository _carRepository;
        public RentalRequestService(IRentalRequestRepository repository, ICarRepository carRepository)
        {
            _repository = repository;
            _carRepository = carRepository;
        }

        public async Task<AddRentalResponseDTO> AddRentalRequestAsync(AddRentalRequestDTO requestDTO)
        {
            // Fetch car details
            var car = await _carRepository.GetCarByIdAsync(requestDTO.CarId);
            if (car == null)
            {
                throw new Exception("Car not found.");
            }

            // Calculate duration
            int duration = (int)(requestDTO.EndDate - requestDTO.StartDate).TotalDays;
            if (duration <= 0)
            {
                throw new Exception("End date must be later than start date.");
            }

            // Calculate total price
            decimal totalPrice = car.PricePerDay * duration;

            // Set default values for action and status
            string action = "Pending";
            string status = "Request";

            // Create RentalRequest entity
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

            // Save rental request
            await _repository.AddRentalRequestAsync(rentalRequest);

            // Return response DTO
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
            // Retrieve the rental request
            var rentalRequest = await _repository.GetByIdAsync(id);
            if (rentalRequest == null)
            {
                throw new KeyNotFoundException($"Rental request with ID {id} not found.");
            }

            // Update the Action field
            rentalRequest.Action = action;

            // Save changes
            await _repository.UpdateAsync(rentalRequest);

            if (action.Equals("Approved", StringComparison.OrdinalIgnoreCase))
            {
                rentalRequest.Status = "Rented";
                var car = await _carRepository.GetCarByIdAsync(rentalRequest.CarId);
                if (car == null)
                {
                    throw new KeyNotFoundException($"Car with ID {rentalRequest.CarId} not found.");
                }

                // Add the totalPrice from the rental request to the car's totalRevenue
                car.OngoingRevenue += rentalRequest.TotalPrice;

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

            if (status == "Returned" && rentalRequest.Car != null)
            {

                car.TotalRevenue += car.OngoingRevenue;
               car.OngoingRevenue = 0;

                await _carRepository.UpdateAsync(car);
            }

            await _repository.UpdateRentalRequestAsync(rentalRequest);
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
                Addresses = r.User.Addresses.Select(a => new AddressDTO
                {
                    Id = a.Id,
                    HouseNo = a.HouseNo,
                    Street1 = a.Street1,
                    Street2 = a.Street2,
                    City = a.City,
                    ZipCode = a.ZipCode,
                    Country = a.Country
                }).ToList(),
                PhoneNumbers = r.User.PhoneNumbers.Select(p => new PhoneNumberDTO
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
                Images = r.Car.Images.Select(i => new ImageDTO
                {
                    Id = i.Id,
                    ImagePath = i.ImagePath
                }).ToList()
            }).ToList();

            return rentalDTOs;
        }
    }
}
