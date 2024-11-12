using DriveX_Backend.DB;
using DriveX_Backend.Entities.Cars;
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
            return await _context.Cars.Include(c => c.Images).FirstOrDefaultAsync(c => c.Id == id);
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
