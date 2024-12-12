using DriveX_Backend.Entities.Cars;
using DriveX_Backend.Entities.RentalRequest;

namespace DriveX_Backend.IRepository
{
    public interface ICarRepository
    {
        Task<Car> AddCarAsync(Car car);
        Task SaveImagesAsync(List<CarImage> images);
        Task<Car> GetCarByIdAsync(Guid id);
        Task<List<Car>> GetAllCarsAsync();
        Task<List<Car>> GetAllCars();
        Task UpdateAsync(Car car);
        Task DeleteAsync(Car car);
        Task<Car> GetByRegNoAsync(string regNo);
        Task<RentalRequest?> GetRentalRequestByCarIdAndStatusAsync(Guid carId, string status);
        Task<(decimal TotalOngoingRevenue, decimal TotalRevenue, int TotalCars, int TotalCustomers)> GetTotalRevenuesAsync();
    }
}
