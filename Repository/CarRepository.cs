using DriveX_Backend.DB;
using DriveX_Backend.Entities.Cars;
using DriveX_Backend.Entities.RentalRequest;
using DriveX_Backend.IRepository;
using Microsoft.EntityFrameworkCore;

namespace DriveX_Backend.Repository
{
    public class CarRepository:ICarRepository
    {
        private readonly AppDbContext _context; 
        public CarRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Car> AddCarAsync(Car car)
        {
            await _context.Cars.AddAsync(car);
            await _context.SaveChangesAsync();
            return car;
        }

        public async Task<Car> GetCarByIdAsync(Guid id)
        {
            return await _context.Cars
           .Include(c => c.Brand)   
           .Include(c => c.Model)  
           .Include(c => c.Images)  
           .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Car>> GetAllCarsAsync()
        {
            return await _context.Cars
                .Include(car => car.Brand)
                .Include(car => car.Model)
                .Include(car => car.Images)
                .ToListAsync();
        }

        public async Task<List<Car>> GetAllCars()
        {
            return await _context.Cars.Include(car => car.Images).ToListAsync();
        }

        public async Task UpdateAsync(Car car)
        {
            if (car == null)
            {
                throw new ArgumentNullException(nameof(car), "Car cannot be null.");
            }

            _context.Cars.Update(car); 
            await _context.SaveChangesAsync();
        }


        public async Task DeleteAsync(Car car)
        {
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
        }

        public async Task<Car> GetByRegNoAsync(string regNo)
        {
            return await _context.Cars.FirstOrDefaultAsync(c => c.RegNo == regNo);
        }

        public async Task<RentalRequest?> GetRentalRequestByCarIdAndStatusAsync(Guid carId, string status)
        {
            return await _context.Set<RentalRequest>()
                .FirstOrDefaultAsync(r => r.CarId == carId && r.Action == status);
        }

        public async Task SaveImagesAsync(List<CarImage> images)
        {
            foreach (var image in images)
            {
                // Ensure each image has a unique ID
                if (image.Id == Guid.Empty)
                {
                    image.Id = Guid.NewGuid(); // Generate a new GUID if not set
                }

                // Check if the image already exists
                var existingImage = await _context.CarImages
                                                  .FirstOrDefaultAsync(i => i.Id == image.Id);
                if (existingImage == null)
                {
                    // Only add the image if it doesn't already exist
                    await _context.CarImages.AddAsync(image);
                }
            }

            await _context.SaveChangesAsync();
        }

    }
}
