using DriveX_Backend.Entities.Users;
using DriveX_Backend.Entities.Users.Models;
using DriveX_Backend.IRepository;
using DriveX_Backend.IServices;
using DriveX_Backend.Helpers;
using DriveX_Backend.Entities.Users.UserDTO;

namespace DriveX_Backend.Services
{
    public class UserServices : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserServices(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> AuthenticateUserAsync(SignInRequest signInRequest)
        {
            if (signInRequest == null)
            {
                throw new ArgumentNullException(nameof(signInRequest), "Sign-in request data cannot be null");
            }

            var user = await _userRepository.AuthenticateUserAsync(signInRequest.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(signInRequest.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            return user;
        }

        public async Task<SignUpResponse> CustomerRegister(SignupRequest signupRequest)
        {
            try
            {
                if (signupRequest == null)
                {
                    throw new ArgumentNullException(nameof(signupRequest), "Signup data cannot be null");
                }

                if (!NICValidator.IsValidNIC(signupRequest.NIC))
                {
                    throw new ArgumentException("Invalid NIC format.", nameof(signupRequest.NIC));
                }

                var existingUserByNIC = await _userRepository.GetUserByNICAsync(signupRequest.NIC);
                if (existingUserByNIC != null)
                {
                    throw new ArgumentException("A user with this NIC already exists.", nameof(signupRequest.NIC));
                }

                if (!EmailValidator.IsValidEmail(signupRequest.Email))
                {
                    throw new ArgumentException("Invalid email format.", nameof(signupRequest.Email));
                }

                var existingUserByEmail = await _userRepository.GetUserByEmailAsync(signupRequest.Email);
                if (existingUserByEmail != null)
                {
                    throw new ArgumentException("A user with this email already exists.", nameof(signupRequest.Email));
                }

                var user = new User
                {
                    FirstName = signupRequest.FirstName,
                    LastName = signupRequest.LastName,
                    Email = signupRequest.Email,
                    NIC = signupRequest.NIC,
                    Password = PasswordValidator.HashPassword(signupRequest.Password),
                    Role = signupRequest.Role,
                };

                var signupCustomer = await _userRepository.AddUserAsync(user);

                var signUpResponse = new SignUpResponse
                {
                    Id = signupCustomer.Id,
                    FirstName = signupCustomer.FirstName,
                    LastName = signupCustomer.LastName,
                    Email = signupCustomer.Email,
                    NIC = signupCustomer.NIC,
                    Role = signupCustomer.Role,
                };

                return signUpResponse;
            }
            catch (ArgumentNullException ex)
            {

                throw new ApplicationException("Invalid input: " + ex.Message, ex);
            }
            catch (ArgumentException ex)
            {

                throw new ApplicationException("Validation error: " + ex.Message, ex);
            }
            catch (Exception ex)
            {

                throw new ApplicationException("An unexpected error occurred during the registration process.", ex);
            }
        }

        public async Task<CustomerResponseDto> GetCustomerById(Guid id)
        {
            var user = await _userRepository.GetCustomerByIdAsync(id);
            if (user == null)
            {
                throw new Exception("Customer not found");
            }

            return new CustomerResponseDto
            {
                Id = user.Id,
                Image = user.Image,
                FirstName = user.FirstName,
                LastName = user.LastName,
                NIC = user.NIC,
                Licence = user.Licence,
                Email = user.Email,
                Addresses = user.Addresses?.Select(a => new AddressDTO
                {
                    HouseNo = a.HouseNo,
                    Street1 = a.Street1,
                    Street2 = a.Street2,
                    City = a.City,
                    ZipCode = a.ZipCode,
                    Country = a.Country,
                }).ToList(),
                PhoneNumbers = user.PhoneNumbers?.Select(n => new PhoneNumberDTO
                {
                    Mobile1 = n.Mobile1,
                    Mobile2 = n.Mobile2,
                }).ToList(),

            };
            
        }

        
    }
}
