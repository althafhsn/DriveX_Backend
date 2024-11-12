using DriveX_Backend.Entities.Users;
using DriveX_Backend.Entities.Users.Models;
using DriveX_Backend.Entities.Users.UserDTO;

namespace DriveX_Backend.IServices
{
    public interface IUserService
    {
        Task<SignUpResponse> CustomerRegister(SignupRequest signupRequest);
        Task<User> AuthenticateUserAsync(SignInRequest signInRequest);
        Task<CustomerResponseDto> GetCustomerById(Guid id);
        Task<List<User>> GetAllUsersAsync();

    }
}
