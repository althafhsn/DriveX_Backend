using DriveX_Backend.Entities.RentalRequest;
using DriveX_Backend.Entities.Users;
using DriveX_Backend.Entities.Users.Models;
using DriveX_Backend.Entities.Users.UserDTO;

namespace DriveX_Backend.IRepository
{
    public interface IUserRepository
    {
        Task<User> AddUserAsync(User user);
        Task<IEnumerable<User>> AddUsersAsync(IEnumerable<User> users);
        Task<User> GetUserByNICAsync(string nic);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> AuthenticateUserAsync(string username);
        Task<User> GetCustomerByIdAsync(Guid id);

        Task<List<User>> GetAllUsersAsync();
        Task<bool> RefreshTokenExistsAsync(string refreshToken);
        Task<bool> UpdateUserRefreshTokenAsync(User user);
        Task UpdateAddressesAsync(Guid userId, List<Address> updatedAddresses);

        Task UpdatePhoneNumbersAsync(Guid userId, List<PhoneNumber> phoneNumbers);
        Task<User> ResetPasswordChange(User user);
        Task<User> ResetPassword(string email);

        Task<List<User>> DashboardAllCustomersAsync();

        Task<User> UpdateCustomerAsync(User customer);
        Task<User> AddCustomerDashboard(User user);
        Task SaveAsync();
        Task<bool> DeleteCustomerAsync(Guid id);
        Task<IEnumerable<RentalRequest>> GetRentalRequestsByCustomerIdAsync(Guid customerId);
        Task<List<User>> GetAllManagersAsync();
        Task<User> UpdateManagerAsync(User manager);
        Task<User> GetManagerByIdAsync(Guid id);
        Task<User> AddManagerDashboard(User user);
        Task<User> GetUserByNICAndRoleAsync(string nic, Role role);
        Task UpdateUserAsync(User user);
        Task<List<Address>> GetAddressesByCustomerIdAsync(Guid customerId);
        Task<Address> AddAddressAsync(Address address);
        /*Task UpdateAddressAsync(Address address);*/
        Task UpdateAddressAsync(Address address);
        Task<Address> GetAddressByIdAsync(Guid Id);

        Task<bool> DeleteAddressAsync(Address address);
        Task<PhoneNumber> AddPhoneNumberAsync(PhoneNumber phoneNumber);
        Task<PhoneNumber> UpdatePhoneNumberAsync(PhoneNumber phoneNumber);
        Task<bool> DeletePhoneNumberAsync(Guid phoneNumberId);
        Task<PhoneNumber?> GetPhoneNumberByIdAsync(Guid phoneNumberId);
        Task<List<PhoneNumber>> GetPhoneNumbersByCustomerIdAsync(Guid customerId);
        Task DeleteManagerAsync(User manager);


    }
}
