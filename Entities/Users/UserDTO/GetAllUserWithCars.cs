using DriveX_Backend.Entities.Cars.Models;

namespace DriveX_Backend.Entities.Users.UserDTO
{
    public class GetAllUserWithCars
    {
        public Guid UserId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NIC { get; set; }

        public string? Licence { get; set; }

        public string Email { get; set; }
        public string Image { get; set; }

        public List<AddressResponseDTO>? Addresses { get; set; }
        public List<PhoneNumberResponseDTO>? PhoneNumbers { get; set; }
        public Guid CarId { get; set; }
        public Guid BrandId { get; set; }
        public string BrandName { get; set; }
        public Guid ModelId { get; set; }
        public string ModelName { get; set; }
        public string RegNo { get; set; }
        public decimal PricePerDay { get; set; }
        public string GearType { get; set; }
        public string FuelType { get; set; }
        public string Mileage { get; set; }
        public string SeatCount { get; set; }
        public string Status { get; set; } = "Available";
        public Guid Id { get; set; }
        public string Action { get; set; }

        public List<CarDTO> RentedCars { get; set; }

    }
}
