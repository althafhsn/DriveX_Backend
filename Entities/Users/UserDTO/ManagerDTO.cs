namespace DriveX_Backend.Entities.Users.UserDTO
{
    public class ManagerDTO
    {
       

        public string? Image { get; set; } = "https://www.freepik.com/free-vector/young-prince-vector-illustration_354177187.htm#fromView=keyword&page=1&position=2&uuid=36b315b4-85fc-40a6-8e58-93c531499cf7";
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NIC { get; set; }
  
        public string Email { get; set; }
        public List<AddressDTO>? Addresses { get; set; }
        public List<PhoneNumberDTO>? PhoneNumbers { get; set; }
        public string? Notes { get; set; }
    }
    public  class UpdateManagerDTO
    {
        public Guid? Id { get; set; }
        public string? Image { get; set; } = "https://www.freepik.com/free-vector/young-prince-vector-illustration_354177187.htm#fromView=keyword&page=1&position=2&uuid=36b315b4-85fc-40a6-8e58-93c531499cf7";
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NIC { get; set; }
        public Role Role { get; set; }
        public string Email { get; set; }
        public List<AddressResponseDTO>? Addresses { get; set; }
        public List<PhoneNumberResponseDTO>? PhoneNumbers { get; set; }
        public string? Notes { get; set; }

    }
    public class DashboardRequestManagerDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string NIC { get; set; }
        public string Password { get; set; }
        public string Notes { get; set; }
        public string? Image { get; set; }
        public List<PhoneNumberDTO> PhoneNumbers { get; set; }
        public List<AddressDTO>? Addresses { get; set; }
    }
    public class DashboardAllManagerDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string NIC { get; set; }
        public string Role { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
        public List<AddressResponseDTO>? Addresses { get; set; }
        public List<PhoneNumberResponseDTO>? PhoneNumbers { get; set; }
    }

}
