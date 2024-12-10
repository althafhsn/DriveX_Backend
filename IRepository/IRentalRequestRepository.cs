using DriveX_Backend.Entities.RentalRequest;
using DriveX_Backend.Entities.Users.UserDTO;

namespace DriveX_Backend.IRepository
{
    public interface IRentalRequestRepository
    {
        Task AddRentalRequestAsync(RentalRequest rentalRequest);
        Task<RentalRequest> GetByIdAsync(Guid id);
        Task UpdateAsync(RentalRequest rentalRequest);
        Task UpdateRentalRequestAsync(RentalRequest rentalRequest);
        Task<List<RentalRequest>> GetAllRentalRequestsAsync();

        Task<RentalRequest?> GetRentalRequestByCarIdAsync(Guid carId);
        Task<IEnumerable<RentalRequest>> GetAllRentalRequestsByCarIdAsync(Guid carId);

        Task<IEnumerable<RentalRequest>> GetRentalRequestsByCustomerIdAsync(Guid customerId);
        Task<List<RentalRequest>> GetAllOngoingRentals();
        Task<List<RentalRequest>> GetAllRenteds();
        Task<List<RentalRequest>> GetAllCancelledRentals();
        Task<List<RentalRequest>> GetRecentRentalRequest();
        Task<List<RentalRequest>> GetOverdueRentalsAsync();
    }
}
