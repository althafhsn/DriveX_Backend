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
    }
}
