using DriveX_Backend.Entities.Cars;
using DriveX_Backend.Entities.Users;

namespace DriveX_Backend.IRepository
{
    public interface IFavouriteRepository
    {
        Task<Favourite> GetFavoriteAsync(Guid userId, Guid carId);
        Task AddFavoriteAsync(Favourite favourite);
        Task<List<Favourite>> GetFavoritesByUserIdAsync(Guid userId);
        Task<bool> DeleteFavouriteAsync(Guid id);
    }
}
