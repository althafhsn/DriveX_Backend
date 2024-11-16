using DriveX_Backend.Entities.Cars;

namespace DriveX_Backend.IRepository
{
    public interface ICarRepository
    {
        Task<Car> AddCarAsync(Car car);
        Task SaveImagesAsync(List<CarImage> images);
        Task<Car> GetCarByIdAsync(Guid id);
        Task<List<Car>> GetAllCarsAsync();
        Task UpdateAsync(Car car);
    }
}
