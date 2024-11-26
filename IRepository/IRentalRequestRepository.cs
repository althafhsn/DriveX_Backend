using DriveX_Backend.Entities.RentalRequest;

namespace DriveX_Backend.IRepository
{
    public interface IRentalRequestRepository
    {
        Task AddRentalRequestAsync(RentalRequest rentalRequest);
        Task<RentalRequest> GetByIdAsync(Guid id);
        Task UpdateAsync(RentalRequest rentalRequest);
        Task UpdateRentalRequestAsync(RentalRequest rentalRequest);
        Task<IEnumerable<RentalRequest>> GetAllRentalRequestsAsync();
    }
}
