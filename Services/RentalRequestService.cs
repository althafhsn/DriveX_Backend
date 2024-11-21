using DriveX_Backend.Entities.RentalRequest;
using DriveX_Backend.Entities.RentalRequest.Models;
using DriveX_Backend.IRepository;
using DriveX_Backend.IServices;

namespace DriveX_Backend.Services
{
    public class RentalRequestService : IRentalRequestService
    {
        private readonly IRentalRequestRepository _rentalRepository;
        public RentalRequestService(IRentalRequestRepository repository)
        {
            _rentalRepository = repository;
        }

        public async Task<AddRentalResponseDTO> AddRentalAsync(AddRentalRequestDTO requestDTO)
        {
            // Validate dates
            if (requestDTO.EndDate <= requestDTO.StartDate)
                throw new ArgumentException("End date must be greater than start date.");

            // Calculate duration
            var duration = CalculateDuration(requestDTO.StartDate, requestDTO.EndDate, requestDTO.PriceType);

            // Calculate total price dynamically based on car and price type
            var totalPrice = await CalculateTotalPriceAsync(requestDTO.CarId, duration, requestDTO.PriceType);

            // Fetch car details
            var carDetails = await _rentalRepository.GetCarDetailsAsync(requestDTO.CarId);
         
            if (carDetails == null)
                throw new ArgumentException("Car not found.");

            // Prepare response DTO
            var response = new AddRentalResponseDTO
            {
                Id = Guid.NewGuid(),
                RequestDate = DateTime.Today,
                Brand = carDetails.Brand,
                Model = carDetails.Model,
                UserId = requestDTO.UserId,
                StartDate = requestDTO.StartDate,
                EndDate = requestDTO.EndDate,
                Duration = duration,
                PriceType = requestDTO.PriceType,
                TotalPrice = totalPrice
            };

            // Map response DTO to Rental entity and save it
            var rentalEntity = MapToRentalEntity(response);
            await _rentalRepository.AddRentalAsync(rentalEntity);

            return response;
        }

        private int CalculateDuration(DateTime startDate, DateTime endDate, string priceType)
        {
            if (priceType.ToLower() == "priceperhour")
            {
                return (int)(endDate - startDate).TotalHours; // Duration in hours
            }
            else // Default to "priceperday"
            {
                int duration = (int)(endDate - startDate).TotalDays;
                return duration == 0 ? 1 : duration; // Minimum rental period is 1 day
            }
        }

        private async Task<decimal> CalculateTotalPriceAsync(Guid carId, int duration, string priceType)
        {
            // Fetch car-specific prices
            var (pricePerDay, pricePerHour) = await _rentalRepository.GetCarPricingAsync(carId);

            // Calculate based on price type
            return priceType.ToLower() switch
            {
                "priceperhour" => duration * pricePerHour,
                "priceperday" => duration * pricePerDay,
                _ => throw new ArgumentException("Invalid price type.")
            };
        }


        private RentalRequest MapToRentalEntity(AddRentalResponseDTO responseDTO)
        {
            return new RentalRequest
            {
                Id = responseDTO.Id,
                RequestDate = responseDTO.RequestDate,
                UserId = responseDTO.UserId,
                CarId = responseDTO.Id, // Ensure correct property is mapped
                StartDate = responseDTO.StartDate,
                EndDate = responseDTO.EndDate,
                Duration = responseDTO.Duration,
                TotalPrice = responseDTO.TotalPrice,
                Action = responseDTO.Action ?? "Pending" // Default action to "Pending" if not specified
            };
        }

    }
}
