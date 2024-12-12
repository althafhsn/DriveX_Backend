using DriveX_Backend.Entities.Cars;

namespace DriveX_Backend.Entities.Users
{
    public class Favourite
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CarId { get; set; }

        public User User { get; set; }
        public Car Car { get; set; }
    }

}
