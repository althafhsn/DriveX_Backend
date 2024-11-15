using DriveX_Backend.Entities.Cars.Models;

namespace DriveX_Backend.IServices
{
    public interface ICarService
    {
        Task<CarDTO> AddCarAsync(CarRequestDTO carRequestDto);
        Task<CarDTO> GetCarByIdAsync(Guid id);
    }
}
