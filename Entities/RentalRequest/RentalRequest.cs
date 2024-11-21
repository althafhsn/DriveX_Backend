using DriveX_Backend.Entities.Cars;
using DriveX_Backend.Entities.Users;

namespace DriveX_Backend.Entities.RentalRequest
{
    public class RentalRequest
    {
        public Guid Id { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Today;
        public Guid CarId { get; set; }
        public Guid UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Duration { get; set; }
        public string PriceType { get; set; }
        public decimal TotalPrice { get; set; }
        public string Action { get; set; }
        public string Status { get; set; }

        public Car Car { get; set; }
        public User User { get; set; }

    }
}
