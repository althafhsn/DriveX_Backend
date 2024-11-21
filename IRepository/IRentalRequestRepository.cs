using DriveX_Backend.Entities.RentalRequest;
using DriveX_Backend.Entities.RentalRequest.Models;

namespace DriveX_Backend.IRepository
{
    public interface IRentalRequestRepository
    {
        Task AddRentalAsync(RentalRequest rental);
        Task<BrandAndModelDTO> GetCarDetailsAsync(Guid carId);
        Task<(decimal PricePerDay, decimal PricePerHour)> GetCarPricingAsync(Guid carId);
    }
}
