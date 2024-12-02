using DriveX_Backend.Entities.Cars.Models;
using DriveX_Backend.Entities.Users;
using DriveX_Backend.Entities.Users.Models;
using DriveX_Backend.Entities.Users.UserDTO;

namespace DriveX_Backend.IServices
{
    public interface IUserService
    {
        Task<SignUpResponse> CustomerRegister(SignupRequest signupRequest);
        Task<TokenApiDTO> AuthenticateUserAsync(SignInRequest signInRequest);
        Task<CustomerResponseDto> GetCustomerById(Guid id);
        Task<List<User>> GetAllUsersAsync();
        Task<TokenApiDTO> Refresh(TokenApiDTO tokenApiDTO);
        Task<EmailModel> SendResetEmail(string email);
        Task<User> ResetPassword(ResetPasswordDTO resetPasswordDTO);
        Task<List<DashboardAllCustomerDTO>> DashboardAllCustomersAsync();
        Task<UpdateUserResponseDTO> UpdateCustomerAsync(Guid id, UpdateUserDTO updateDTO);

        Task<DashboardAllCustomerDTO> AddCustomerDashboard(DashboardRequestCustomerDTO dashboardRequestCustomerDTO);
        Task<bool> DeleteCustomerAsync(Guid id);
        Task<(CustomerResponseDto customer, List<CarCustomerDTO>? rentedCars, string message)> GetCustomerDetailsWithRentalInfoAsync(Guid customerId);

        Task<bool> ChangePasswordAsync(UpdatePasswordDTO updatePasswordDTO);

    }
}
