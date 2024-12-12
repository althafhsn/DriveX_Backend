using DriveX_Backend.Entities.Cars.Models;

namespace DriveX_Backend.IServices
{
    public interface ICarService
    {
        Task<CarDTO> AddCarAsync(CarRequestDTO carRequestDto);
        Task<CarDTO> GetCarByIdAsync(Guid id);
        Task<CarCustomerDTO> GetCarById(Guid id);
        Task<List<CarDTO>> GetAllCarsAsync();
        Task<List<CarSummaryDTO>> GetAllCars();
        Task<CarDTO> UpdateCarAsync(Guid id, UpdateCarDTO updateCarDto);
        Task<bool> DeleteCarAsync(Guid id);
       
        Task<(CarDTO car, List<UserDTO> customers, string message)> GetCarDetailsWithRentalInfoAsync(Guid carId);
        Task<(decimal TotalOngoingRevenue, decimal TotalRevenue, int TotalCars, int TotalCustomers)> GetTotalRevenuesAsync();
    }
}
