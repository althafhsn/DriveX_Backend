using DriveX_Backend.DB;
using DriveX_Backend.Entities.RentalRequest;
using DriveX_Backend.Entities.RentalRequest.Models;
using DriveX_Backend.IRepository;
using Microsoft.EntityFrameworkCore;

namespace DriveX_Backend.Repository
{
    public class RentalRequestRepository:IRentalRequestRepository
    {
        private readonly AppDbContext _dbContext;
        public RentalRequestRepository(AppDbContext appDbContext)
        {
            _dbContext = appDbContext;
        }

        public async Task<BrandAndModelDTO> GetCarDetailsAsync(Guid carId)
        {
            return await _dbContext.Cars.Where(c => c.Id == carId).Select(c => new BrandAndModelDTO
                {
                    Brand = c.Brand.Name, 
                    Model = c.Model.Name  
                }).FirstOrDefaultAsync();
        }

        public async Task<(decimal PricePerDay, decimal PricePerHour)> GetCarPricingAsync(Guid carId)
        {
            var result = await _dbContext.Cars
                .Where(c => c.Id == carId)
                .Select(c => new { c.PricePerDay, c.PricePerHour }) // Anonymous object
                .FirstOrDefaultAsync();

            if (result == null)
                throw new ArgumentException("Car not found.");

            return (result.PricePerDay, result.PricePerHour); // Convert to tuple in memory
        }


        public async Task AddRentalAsync(RentalRequest rental)
        {
            try
            {
                await _dbContext.RentalRequests.AddAsync(rental);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException?.Message ?? ex.Message;
                Console.WriteLine($"Error saving rental request: {innerException}");
                throw new Exception("An error occurred while saving the rental request. See details in the log.");
            }

        }
    }
}
