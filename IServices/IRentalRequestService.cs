using DriveX_Backend.Entities.RentalRequest;
using DriveX_Backend.Entities.RentalRequest.Models;
using DriveX_Backend.Entities.Users.UserDTO;

namespace DriveX_Backend.IServices
{
    public interface IRentalRequestService
    {
        Task<AddRentalResponseDTO> AddRentalRequestAsync(AddRentalRequestDTO requestDTO);
        Task UpdateRentalActionAsync(Guid id, string action);
        Task UpdateRentalStatusAsync(Guid id, string status);
        Task<List<GetAllRentalDTO>> GetAllRentalRequestsAsync();
        Task<List<OngoingRentalsDTO>> GetAllOngoingRentals();
        Task<List<OngoingRentalsDTO>> GetAllRented();
        Task<List<OngoingRentalsDTO>> GetAllCancelledRentals();
        Task<List<OverDueRentalsDTO>> GetAllOverDueRentals();
        Task<List<recentRentalRequestDTO>> GetRecentRentalRequests();
        Task<List<getCustomerRentalDTO>> GetRentalRequestsByCustomerIdAsync(Guid customerId);
        Task ActionCancelByCustomer(Guid id, string action);
    }
}
