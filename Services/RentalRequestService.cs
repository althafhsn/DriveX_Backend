using DriveX_Backend.Entities.RentalRequest.Models;
using DriveX_Backend.Entities.RentalRequest;
using DriveX_Backend.IRepository;
using DriveX_Backend.IServices;
using DriveX_Backend.Repository;

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
                StartDate = rentalRequest.StartDate,
                EndDate = rentalRequest.EndDate,
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
        }

        public async Task UpdateRentalStatusAsync(Guid id, string status)
        {
            // Validate the status value (this could be adjusted based on your logic)
            if (string.IsNullOrEmpty(status))
            {
                throw new ArgumentException("Status cannot be empty.");
            }

            var rentalRequest = await _repository.GetByIdAsync(id);

            if (rentalRequest == null)
            {
                throw new KeyNotFoundException("Rental request not found.");
            }

            // Update the status of the rental request
            rentalRequest.Status = status;

            // Save the changes to the database
            await _repository.UpdateRentalRequestAsync(rentalRequest);
        }

        public async Task<IEnumerable<GetAllRentalRequestDTO>> GetAllRentalRequestsAsync()
        {
            var rentalRequests = await _repository.GetAllRentalRequestsAsync();

            // Map the RentalRequest entities to GetAllRentalRequestDTO
            var rentalRequestDtos = rentalRequests.Select(r => new GetAllRentalRequestDTO
            {
                Id = r.Id,
                CarId = r.CarId,
                UserId = r.UserId,
                RequestDate = r.RequestDate,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                Action = r.Action,
                Status = r.Status
            }).ToList();

            return rentalRequestDtos;
        }
    }
}
