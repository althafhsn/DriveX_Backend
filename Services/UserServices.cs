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
using DriveX_Backend.Repository;

namespace DriveX_Backend.Services
{
    public class UserServices : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly string _jwtSecret = "your-very-secure-key-with-at-least-32-characters";

        public UserServices(IUserRepository userRepository, IConfiguration configuration, IEmailService emailService)
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
            await _userRepository.UpdateUserRefreshTokenAsync(user);


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
                        Image = u.Image ?? string.Empty,  // Handle potential null image paths
                        NIC = u.NIC ?? "N/A",            // Handle potential null values
                        Licence = u.Licence ?? "N/A",    // Handle potential null values
                        Email = u.Email ?? "N/A",
                        Notes = u.Notes ?? "N/A",
                        Addresses = u.Addresses != null
                            ? u.Addresses.Select(a => new AddressResponseDTO
                            {
                                // Map Address fields to AddressResponseDTO fields here
                                HouseNo = a.HouseNo,
                                Street1 = a.Street1,
                                Street2 = a.Street2,
                                City = a.City,
                                ZipCode = a.ZipCode,
                                Country = a.Country
                            }).ToList()
                            : new List<AddressResponseDTO>(), // Return an empty list if null
                        PhoneNumbers = u.PhoneNumbers != null ?
                        u.PhoneNumbers.Select(a => new PhoneNumberResponseDTO
                        {
                            Mobile1 = a.Mobile1,
                        }).ToList() : new List<PhoneNumberResponseDTO>()
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                // Log the exception here if needed
                throw new Exception("Error in UserService: Unable to retrieve customer data.", ex);
            }
        }
        public async Task<UpdateUserResponseDTO> UpdateCustomerAsync(Guid id, UpdateUserDTO updateDTO)
        {
            if (updateDTO == null)
            {
                throw new ArgumentNullException(nameof(updateDTO), "Customer data cannot be null.");
            }

            // Validate NIC
            if (!NICValidator.IsValidNIC(updateDTO.NIC))
            {
                throw new ArgumentException("Invalid NIC format.", nameof(updateDTO.NIC));
            }

            // Validate Email
            if (!EmailValidator.IsValidEmail(updateDTO.Email))
            {
                throw new ArgumentException("Invalid email format.", nameof(updateDTO.Email));
            }

            // Retrieve the existing customer
            var existingCustomer = await _userRepository.GetCustomerByIdAsync(id);
            if (existingCustomer == null)
            {
                throw new KeyNotFoundException("Customer not found.");
            }

            // Check for NIC conflict
            var customerWithSameNIC = await _userRepository.GetUserByNICAsync(updateDTO.NIC);
            if (customerWithSameNIC != null && customerWithSameNIC.Id != id)
            {
                throw new ArgumentException("A customer with this NIC already exists.", nameof(updateDTO.NIC));
            }

            // Check for Email conflict
            var customerWithSameEmail = await _userRepository.GetUserByEmailAsync(updateDTO.Email);
            if (customerWithSameEmail != null && customerWithSameEmail.Id != id)
            {
                throw new ArgumentException("A customer with this email already exists.", nameof(updateDTO.Email));
            }

            // Update customer properties
            existingCustomer.FirstName = updateDTO.FirstName;
            existingCustomer.LastName = updateDTO.LastName;
            existingCustomer.Image = updateDTO.Image;
            existingCustomer.NIC = updateDTO.NIC;
            existingCustomer.Licence = updateDTO.Licence;
            existingCustomer.Email = updateDTO.Email;
            existingCustomer.Notes = updateDTO.Notes;

            // Update existing Addresses if provided
            if (updateDTO.Addresses != null && updateDTO.Addresses.Any())
            {
                // Update or add new addresses based on HouseNo
                foreach (var addressDTO in updateDTO.Addresses)
                {
                    var existingAddress = existingCustomer.Addresses
                        .FirstOrDefault(a => a.HouseNo == addressDTO.HouseNo); // Match by HouseNo

                    if (existingAddress != null)
                    {
                        // Update the existing address with the new data
                        existingAddress.Street1 = addressDTO.Street1;
                        existingAddress.Street2 = addressDTO.Street2;
                        existingAddress.City = addressDTO.City;
                        existingAddress.ZipCode = addressDTO.ZipCode;
                        existingAddress.Country = addressDTO.Country;
                    }
                    else
                    {
                        // If no existing address matches, add a new one
                        existingCustomer.Addresses.Add(new Address
                        {
                            HouseNo = addressDTO.HouseNo,
                            Street1 = addressDTO.Street1,
                            Street2 = addressDTO.Street2,
                            City = addressDTO.City,
                            ZipCode = addressDTO.ZipCode,
                            Country = addressDTO.Country
                        });
                    }
                }
            }

            // Update existing PhoneNumbers if provided
            if (updateDTO.PhoneNumbers != null && updateDTO.PhoneNumbers.Any())
            {
                // Update or add new phone numbers based on Mobile1
                foreach (var phoneDTO in updateDTO.PhoneNumbers)
                {
                    var existingPhoneNumber = existingCustomer.PhoneNumbers
                        .FirstOrDefault(p => p.Mobile1 == phoneDTO.Mobile1); // Match by Mobile1

                    if (existingPhoneNumber != null)
                    {
                        // Update the existing phone number
                        existingPhoneNumber.Mobile1 = phoneDTO.Mobile1;
                    }
                    else
                    {
                        // If no existing phone number matches, add a new one
                        existingCustomer.PhoneNumbers.Add(new PhoneNumber
                        {
                            Mobile1 = phoneDTO.Mobile1
                        });
                    }
                }
            }

            // Save changes to the database
            await _userRepository.SaveAsync();

            // Return the updated customer response DTO
            return new UpdateUserResponseDTO
            {
                Id = existingCustomer.Id,
                FirstName = existingCustomer.FirstName,
                LastName = existingCustomer.LastName,
                Image = existingCustomer.Image,
                NIC = existingCustomer.NIC,
                Licence = existingCustomer.Licence,
                Email = existingCustomer.Email,
                Notes = existingCustomer.Notes,
                Addresses = existingCustomer.Addresses.Select(a => new AddressResponseDTO
                {
                    Id = a.Id,
                    HouseNo = a.HouseNo,
                    Street1 = a.Street1,
                    Street2 = a.Street2,
                    City = a.City,
                    ZipCode = a.ZipCode,
                    Country = a.Country
                }).ToList(),
                PhoneNumbers = existingCustomer.PhoneNumbers.Select(p => new PhoneNumberResponseDTO
                {
                    Id = p.Id,
                    Mobile1 = p.Mobile1,
                    // Include other properties if necessary
                }).ToList()
            };
        }













        public async Task<DashboardAllCustomerDTO> AddCustomerDashboard(DashboardRequestCustomerDTO dashboardRequestCustomerDTO)
        {
            if (dashboardRequestCustomerDTO == null)
            {
                throw new ArgumentNullException(nameof(dashboardRequestCustomerDTO), "Customer data cannot be null");
            }
            if (!NICValidator.IsValidNIC(dashboardRequestCustomerDTO.NIC))
            {
                throw new ArgumentException("Invalid NIC format.", nameof(dashboardRequestCustomerDTO.NIC));
            }

            if (!EmailValidator.IsValidEmail(dashboardRequestCustomerDTO.Email))
            {
                throw new ArgumentException("Invalid email format.", nameof(dashboardRequestCustomerDTO.Email));
            }

            // Check for existing customers
            var existingCustomer = await _userRepository.GetUserByNICAsync(dashboardRequestCustomerDTO.NIC);
            if (existingCustomer != null)
            {
                throw new ArgumentException("A customer with this NIC already exists.", nameof(dashboardRequestCustomerDTO.NIC));
            }

            // Map the DTO to the entity
            var newCustomer = new User
            {
                FirstName = dashboardRequestCustomerDTO.FirstName,
                LastName = dashboardRequestCustomerDTO.LastName,
                Image = dashboardRequestCustomerDTO.Image,
                NIC = dashboardRequestCustomerDTO.NIC,
                Licence = dashboardRequestCustomerDTO.Licence,
                Email = dashboardRequestCustomerDTO.Email,
                Notes = dashboardRequestCustomerDTO.Notes,
                Password = PasswordValidator.HashPassword(dashboardRequestCustomerDTO.Password),
                Addresses = dashboardRequestCustomerDTO.Addresses?.Select(a => new Address
                {
                    HouseNo = a.HouseNo,
                    Street1 = a.Street1,
                    Street2 = a.Street2,
                    City = a.City,
                    ZipCode = a.ZipCode,
                    Country = a.Country
                }).ToList(),
                PhoneNumbers = dashboardRequestCustomerDTO.PhoneNumbers?.Select(p => new PhoneNumber
                {
                    Mobile1 = p.Mobile1,
                }).ToList()
            };

            // Save the customer to the repository
            var createdCustomer = await _userRepository.AddCustomerDashboard(newCustomer);

            // Map the created customer back to the DTO
            return new DashboardAllCustomerDTO
            {
                Id = createdCustomer.Id,
                FirstName = createdCustomer.FirstName,
                LastName = createdCustomer.LastName,
                Email = createdCustomer.Email,
                Licence = createdCustomer.Licence,
                Image = createdCustomer.Image,
                NIC = createdCustomer.NIC,
                Notes = createdCustomer.Notes,
                Addresses = createdCustomer.Addresses?.Select(a => new AddressResponseDTO
                {
                    Id = Guid.NewGuid(),
                    HouseNo = a.HouseNo,
                    Street1 = a.Street1,
                    Street2 = a.Street2,
                    City = a.City,
                    ZipCode = a.ZipCode,
                    Country = a.Country
                }).ToList(),
                PhoneNumbers = createdCustomer.PhoneNumbers?.Select(p => new PhoneNumberResponseDTO
                {
                    Id = Guid.NewGuid(),
                    Mobile1 = p.Mobile1,
                }).ToList()
            };


        }
        public async Task<bool> DeleteCustomerAsync(Guid id)
        {
            var customerDeleted = await _userRepository.DeleteCustomerAsync(id);
            if (!customerDeleted)
            {
                throw new KeyNotFoundException("Customer not found.");
            }

            return true; // Customer deleted successfully
        }
    }

}




