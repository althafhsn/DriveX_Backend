using DriveX_Backend.Entities.Users;
using DriveX_Backend.Entities.Users.Models;
using DriveX_Backend.IRepository;
using DriveX_Backend.IServices;
using DriveX_Backend.Helpers;
using DriveX_Backend.Entities.Users.UserDTO;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http.HttpResults;
using DriveX_Backend.Utility;
using Org.BouncyCastle.Crypto.Fpe;

namespace DriveX_Backend.Services
{
    public class UserServices : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly string _jwtSecret = "your-very-secure-key-with-at-least-32-characters";

        public UserServices(IUserRepository userRepository , IConfiguration configuration , IEmailService emailService)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<TokenApiDTO> AuthenticateUserAsync(SignInRequest signInRequest)
        {
            if (signInRequest == null)
            {
                throw new ArgumentNullException(nameof(signInRequest), "Sign-in request data cannot be null");
            }

            var user = await _userRepository.AuthenticateUserAsync(signInRequest.Username);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            if (!BCrypt.Net.BCrypt.Verify(signInRequest.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            var accessToken = CreateJwt(user);
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ApplicationException("Failed to generate access token");
            }

            var refreshToken = CreateRefreshToken();
            user.RefreshToken = refreshToken;


            await _userRepository.UpdateUserRefreshTokenAsync(user);

            return new TokenApiDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

  

        public async Task<TokenApiDTO> Refresh(TokenApiDTO tokenApiDTO)
        {
            if (tokenApiDTO == null || string.IsNullOrEmpty(tokenApiDTO.AccessToken) || string.IsNullOrEmpty(tokenApiDTO.RefreshToken))
            {
                throw new ArgumentException("Invalid token data provided.");
            }

            string accessToken = tokenApiDTO.AccessToken;
            string refreshToken = tokenApiDTO.RefreshToken;

       
            var principal = GetPrincipalFromExpiredToken(accessToken);
            if (principal == null || string.IsNullOrEmpty(principal.Identity?.Name))
            {
                throw new SecurityTokenException("Invalid access token.");
            }

            var username = principal.Identity.Name;

          
            var user = await _userRepository.AuthenticateUserAsync(username);
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiry <= DateTime.UtcNow)
            {
                throw new SecurityTokenException("Invalid or expired refresh token.");
            }

        
            var newAccessToken = CreateJwt(user);
            var newRefreshToken = CreateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1); 
            await _userRepository.UpdateUserRefreshTokenAsync(user) ;

         
            return new TokenApiDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
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

        public async Task<EmailModel?> SendResetEmail(string email)

        {

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));

            // Fetch the user by email
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
                return null;

            // Generate a secure token
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var emailToken = Convert.ToBase64String(tokenBytes);

            // Update user with token and expiry
            user.ForgetPasswordToken = emailToken;
            user.ForgetPasswordTokenExpiry = DateTime.UtcNow.AddMinutes(15);

            // Save changes
            await _userRepository.ResetPasswordChange(user);

            // Send email
            string from = _configuration["EmailSettings:From"];
            var emailModel = new EmailModel(email, "Reset Password!",
                ResetEmailBody.ResetPasswordEmailStringBody(email, emailToken));

            _emailService.SendPasswordResetEmail(emailModel);

            return emailModel;
        }



        public async Task<User> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            if (resetPasswordDTO == null)
                throw new ArgumentNullException(nameof(resetPasswordDTO));

            var newToken = resetPasswordDTO.EmailToken.Replace(" ", "+");

            // Retrieve user by email
            var user = await _userRepository.GetUserByEmailAsync(resetPasswordDTO.Email);
            if (user == null)
                throw new Exception("User not found.");

            // Validate token and expiry
            if (user.ForgetPasswordToken != newToken || user.ForgetPasswordTokenExpiry < DateTime.UtcNow)
                throw new Exception("Invalid or expired reset token.");

            // Validate passwords
            if (resetPasswordDTO.NewPassword != resetPasswordDTO.ConfirmPassword)
                throw new Exception("Passwords do not match.");

            // Update user password
            user.Password = PasswordValidator.HashPassword(resetPasswordDTO.NewPassword);

            // Clear reset token and expiry
            user.ForgetPasswordToken = null;
            user.ForgetPasswordTokenExpiry = null;

            // Save changes
            await _userRepository.ResetPasswordChange(user);

            return user;
        }


        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        private string CreateJwt(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            string userNameClaim = !string.IsNullOrEmpty(user.NIC) ? user.NIC :
                                   !string.IsNullOrEmpty(user.Licence) ? user.Licence :
                                   user.Email;

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.Name, userNameClaim),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = credentials,
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
        private string CreateRefreshToken()
        {
            while (true)
            {
                var tokenBytes = RandomNumberGenerator.GetBytes(64);
                var refreshToken = Convert.ToBase64String(tokenBytes);


                var tokenExists = _userRepository.RefreshTokenExistsAsync(refreshToken).Result;
                if (!tokenExists)
                {
                    return refreshToken;
                }
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var key = Encoding.ASCII.GetBytes(_jwtSecret);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        public async Task<List<DashboardAllCustomerDTO>> DashboardAllCustomersAsync()
        {
            try
            {
                var users = await _userRepository.DashboardAllCustomersAsync();

                if (users == null)
                {
                    return new List<DashboardAllCustomerDTO>();
                }

                return users
                    .Where(u => u.Role == Role.Customer)
                    .Select(u => new DashboardAllCustomerDTO
                    {
                        Id = u.Id,
                        FirstName = u.FirstName ?? "N/A", // Handle potential null values
                        LastName = u.LastName ?? "N/A",   // Handle potential null values
                        Image = u.Image ?? string.Empty  // Handle potential null image paths
                    }).ToList();
            }
            catch (Exception ex)
            {
                // Log the exception here if needed
                throw new Exception("Error in UserService: Unable to retrieve customer data.", ex);
            }
        }




    }
}
