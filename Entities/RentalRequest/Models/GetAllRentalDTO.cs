using DriveX_Backend.Entities.Cars.Models;
using DriveX_Backend.Entities.Users;

namespace DriveX_Backend.Entities.RentalRequest.Models
{
    public class GetAllRentalDTO
    {
        public Guid Id { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Today;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Duration { get; set; }
        public decimal TotalPrice { get; set; }
        public string Action { get; set; }
        public string Status { get; set; }
        public Guid UserId { get; set; }
        public string? Image { get; set; } = "https://www.freepik.com/free-vector/young-prince-vector-illustration_354177187.htm#fromView=keyword&page=1&position=2&uuid=36b315b4-85fc-40a6-8e58-93c531499cf7";
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NIC { get; set; }

        public string? Licence { get; set; }

        public string Email { get; set; }

        public List<UserAddressDTO>? Addresses { get; set; }
        public List<UserPhoneNumberDTO>? PhoneNumbers { get; set; }
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
        public List<ImageDTO> Images { get; set; }
        public string carStatus {  get; set; }  
    }

    public class UserAddressDTO
    {
        public Guid Id { get; set; }
        public string HouseNo { get; set; }
        public string Street1 { get; set; }
        public string? Street2 { get; set; }
        public string City { get; set; }
        public int ZipCode { get; set; }
        public string Country { get; set; }
    }

    public class UserPhoneNumberDTO
    {
        public Guid Id { get; set; }
        public string Mobile1 { get; set; }
    }
}
