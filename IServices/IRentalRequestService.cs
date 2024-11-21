using DriveX_Backend.Entities.RentalRequest;
using DriveX_Backend.Entities.RentalRequest.Models;

namespace DriveX_Backend.IServices
{
    public interface IRentalRequestService
    {
        Task<AddRentalResponseDTO> AddRentalRequestAsync(AddRentalRequestDTO requestDTO);
        Task UpdateRentalActionAsync(Guid id, string action);
        Task UpdateRentalStatusAsync(Guid id, string status);
        Task<IEnumerable<GetAllRentalRequestDTO>> GetAllRentalRequestsAsync();
    }
}
