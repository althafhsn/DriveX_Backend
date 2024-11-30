using DriveX_Backend.Entities.Users;
using DriveX_Backend.Entities.Users.UserDTO;

namespace DriveX_Backend.Entities.Cars.Models
{
    public class CarDetailsWithUserDTO
    {
        public Guid Id { get; set; }
        public string RegNo { get; set; }
        public decimal PricePerDay { get; set; }
        public int Year { get; set; }
        public string GearType { get; set; }
        public string FuelType { get; set; }
        public string Mileage { get; set; }
        public string SeatCount { get; set; }
        public List<ImageDTO> Images { get; set; }
        public string BrandName { get; set; }
        public string ModelName { get; set; }
        public UserDTO Customer { get; set; }
        public decimal TotalRevenue { get; set; }
        public string Status { get; set; }
    }

    public class UserDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string NIC { get; set; }
        public List<PhoneNumberResponseDTO>? PhoneNumbers { get; set; }
    }
}
