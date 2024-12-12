using DriveX_Backend.Entities.Cars;
using DriveX_Backend.Entities.Users;
using DriveX_Backend.Entities.Users.FavouriteDTO;
using DriveX_Backend.IRepository;
using DriveX_Backend.IServices;
using DriveX_Backend.Repository;

namespace DriveX_Backend.Services
{
    public class FavouriteService:IFavouriteService
    {
        private readonly IFavouriteRepository _repository;
        private readonly ICarRepository _carRepository;
        public FavouriteService(IFavouriteRepository repository, ICarRepository carRepository)
        {
            _repository = repository;
            _carRepository = carRepository;
        }
        public async Task<AddFavouriteResponseDTO> AddToFavoritesAsync(Guid userId, Guid carId)
        {
            // Check if the car exists
            var car = await _carRepository.GetCarByIdAsync(carId);
            if (car == null)
            {
                throw new ArgumentException("Car not found.");
            }

            // Check if the favorite already exists
            var existingFavorite = await _repository.GetFavoriteAsync(userId, carId);
            if (existingFavorite != null)
            {
                throw new InvalidOperationException("This car is already in your favorites.");
            }

            // Add to favorites
            var favorite = new Favourite
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CarId = carId
            };

            await _repository.AddFavoriteAsync(favorite);

            // Return response DTO
            return new AddFavouriteResponseDTO
            {
                Id = favorite.Id,  // Use the actual ID of the created favorite
                CarId = carId,
                UserId = userId
            };
        }

        public async Task<List<AddFavouriteResponseDTO>> GetFavoritesByUserIdAsync(Guid userId)
        {
            var favourites = await _repository.GetFavoritesByUserIdAsync(userId);

            return favourites.Select(f => new AddFavouriteResponseDTO
            {
                Id = f.Id,
                UserId = f.UserId,
                CarId = f.CarId
            }).ToList();
        }


        public async Task<bool> DeleteFavouriteAsync(Guid id)
        {
            // Call the repository to delete the favourite by id
            return await _repository.DeleteFavouriteAsync(id);
        }

    }
}
