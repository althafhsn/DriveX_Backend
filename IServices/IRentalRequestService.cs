using DriveX_Backend.Entities.RentalRequest.Models;

namespace DriveX_Backend.IServices
{
    public interface IRentalRequestService
    {
        Task<AddRentalResponseDTO> AddRentalAsync(AddRentalRequestDTO requestDTO);
    }
}
