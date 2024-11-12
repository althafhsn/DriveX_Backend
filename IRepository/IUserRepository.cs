using DriveX_Backend.Entities.Users;

namespace DriveX_Backend.IRepository
{
    public interface IUserRepository
    {
        Task<User> AddUserAsync(User user);
        Task<IEnumerable<User>> AddUsersAsync(IEnumerable<User> users);
        Task<User> GetUserByNICAsync(string nic);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> AuthenticateUserAsync(string username);
        Task<User> GetCustomerByIdAsync(Guid id);

        Task<List<User>> GetAllUsersAsync();


    }
}
