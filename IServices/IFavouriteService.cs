using DriveX_Backend.Entities.Cars;
using DriveX_Backend.Entities.Users;
using DriveX_Backend.Entities.Users.FavouriteDTO;

namespace DriveX_Backend.IServices
{
    public interface IFavouriteService
    {
        Task<AddFavouriteResponseDTO> AddToFavoritesAsync(Guid userId, Guid carId);
        Task<List<AddFavouriteResponseDTO>> GetFavoritesByUserIdAsync(Guid userId);
        Task<bool> DeleteFavouriteAsync(Guid id);

    }
}
