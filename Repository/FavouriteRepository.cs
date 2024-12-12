using DriveX_Backend.DB;
using DriveX_Backend.Entities.Cars;
using DriveX_Backend.Entities.Users;
using DriveX_Backend.IRepository;
using Microsoft.EntityFrameworkCore;

namespace DriveX_Backend.Repository
{
    public class FavouriteRepository:IFavouriteRepository
    {
        private readonly AppDbContext _context;
        public FavouriteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Favourite> GetFavoriteAsync(Guid userId, Guid carId)
        {
            return await _context.Favourites.FirstOrDefaultAsync(f => f.UserId == userId && f.CarId == carId);
        }

        public async Task AddFavoriteAsync(Favourite favourite)
        {
            await _context.Favourites.AddAsync(favourite);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Favourite>> GetFavoritesByUserIdAsync(Guid userId)
        {
            return await _context.Favourites.Where(f => f.UserId == userId).ToListAsync();
        }

        public async Task<bool> DeleteFavouriteAsync(Guid id)
        {
            // Find the favourite by id
            var favourite = await _context.Favourites
                .FirstOrDefaultAsync(f => f.Id == id);

            if (favourite == null)
            {
                return false; // Favourite not found
            }

            // Remove the favourite from the database
            _context.Favourites.Remove(favourite);
            await _context.SaveChangesAsync();

            return true; // Successfully deleted
        }
    }


}
