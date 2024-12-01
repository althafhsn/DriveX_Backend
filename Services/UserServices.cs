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
using DriveX_Backend.Entities.Cars.Models;
using DriveX_Backend.Entities.Cars;
using DriveX_Backend.Entities.RentalRequest;

namespace DriveX_Backend.Services
{
    public class UserServices : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IRentalRequestRepository _rentalRequestRepository;
        private readonly string _jwtSecret = "your-very-secure-key-with-at-least-32-characters";

        public UserServices(IUserRepository userRepository, IConfiguration configuration, IEmailService emailService, IRentalRequestRepository rentalRequestRepository)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _emailService = emailService;
            _rentalRequestRepository = rentalRequestRepository;
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
                                Id = a.Id,
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
                        { Id = a.Id,
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
            existingCustomer.status = updateDTO.Status;

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


        //public async Task<List<AddressResponseDTO>> UpdateAddressAsync(Guid id, List<AddressResponseDTO> addressDTOs)

        //{
        //    // Fetch existing addresses for the customer
        //    var existingAddresses = await _userRepository.GetCustomerByIdAsync(id);

        //    if (existingAddresses == null)
        //    {
        //        return null; // Address record not found
        //    }
        //    if (addressDTOs.Count > 2)
        //    {
        //        throw new InvalidOperationException("A user cannot have more than two addresses.");
        //    }
        //    // Map DTOs to entity
        //    var updatedAddresses = addressDTOs.Select(dto => new Address
        //    {
        //        Id = Guid.NewGuid(), // Generate new IDs for updated addresses
        //        UserId = id,
        //        HouseNo = dto.HouseNo,
        //        Street1 = dto.Street1,
        //        Street2 = dto.Street2,
        //        City = dto.City,
        //        ZipCode = dto.ZipCode,
        //        Country = dto.Country
        //    }).ToList();

        //    if (existingAddresses.Addresses.Count + updatedAddresses.Count > 2)
        //    {
        //        throw new InvalidOperationException("A user cannot have more than two addresses.");
        //    }


        //    // Update addresses in repository
        //    await _userRepository.UpdateAddressesAsync(id, updatedAddresses);

        //    // Map updated entities to response DTO
        //    var addressResponseDTOs = updatedAddresses.Select(a => new AddressResponseDTO
        //    {
        //        Id = a.Id,
        //        HouseNo = a.HouseNo,
        //        Street1 = a.Street1,
        //        Street2 = a.Street2,
        //        City = a.City,
        //        ZipCode = a.ZipCode,
        //        Country = a.Country
        //    }).ToList();

        //    return addressResponseDTOs;
        //}


        public async Task<List<AddressResponseDTO>> UpdateAddressAsync(Guid userId, List<AddressResponseDTO> addressDTOs)
        {
            // Fetch the existing addresses for the user
            var existingUser = await _userRepository.GetCustomerByIdAsync(userId);
            if (existingUser == null)
            {
                throw new InvalidOperationException($"User with ID {userId} not found.");
            }

            var existingAddresses = existingUser.Addresses.ToList();

            // Ensure the total number of addresses does not exceed 2
            if (addressDTOs.Count > 2)
            {
                throw new InvalidOperationException("A user cannot have more than two addresses.");
            }

            var updatedAddresses = new List<Address>();

            foreach (var dto in addressDTOs)
            {
                if (dto.Id == Guid.Empty)
                {
                    // Adding a new address if there's room
                    if (existingAddresses.Count < 2)
                    {
                        updatedAddresses.Add(new Address
                        {
                            Id = Guid.NewGuid(),
                            UserId = userId,
                            HouseNo = dto.HouseNo,
                            Street1 = dto.Street1,
                            Street2 = dto.Street2,
                            City = dto.City,
                            ZipCode = dto.ZipCode,
                            Country = dto.Country
                        });
                    }
                    else
                    {
                        throw new InvalidOperationException("Cannot add more than two addresses.");
                    }
                }
                else
                {
                    // Updating an existing address
                    var existingAddress = existingAddresses.FirstOrDefault(a => a.Id == dto.Id);
                    if (existingAddress != null)
                    {
                        existingAddress.HouseNo = dto.HouseNo;
                        existingAddress.Street1 = dto.Street1;
                        existingAddress.Street2 = dto.Street2;
                        existingAddress.City = dto.City;
                        existingAddress.ZipCode = dto.ZipCode;
                        existingAddress.Country = dto.Country;

                        updatedAddresses.Add(existingAddress);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Address with ID {dto.Id} does not exist.");
                    }
                }
            }

            // Ensure the updated addresses count does not exceed 2
            if (updatedAddresses.Count > 2)
            {
                throw new InvalidOperationException("A user cannot have more than two addresses.");
            }

            // Save the changes to the repository
            await _userRepository.UpdateAddressesAsync(userId, updatedAddresses);

            // Map updated entities to response DTOs
            return updatedAddresses.Select(address => new AddressResponseDTO
            {
                Id = address.Id,
                HouseNo = address.HouseNo,
                Street1 = address.Street1,
                Street2 = address.Street2,
                City = address.City,
                ZipCode = address.ZipCode,
                Country = address.Country
            }).ToList();
        }



        public async Task<List<PhoneNumberResponseDTO>> UpdatePhoneNumberAsync(Guid userId, List<PhoneNumberResponseDTO> phoneNumberDTOs)
        {
            // Fetch the existing phone numbers for the user
            var existingUser = await _userRepository.GetCustomerByIdAsync(userId);
            if (existingUser == null)
            {
                throw new InvalidOperationException($"User with ID {userId} not found.");
            }

            var existingPhoneNumbers = existingUser.PhoneNumbers.ToList();

            // Ensure the total number of phone numbers does not exceed 2
            if (phoneNumberDTOs.Count > 2)
            {
                throw new InvalidOperationException("A user cannot have more than two phone numbers.");
            }

            var updatedPhoneNumbers = new List<PhoneNumber>();

            foreach (var dto in phoneNumberDTOs)
            {
                if (dto.Id == Guid.Empty)
                {
                    // Adding a new phone number if there's room
                    if (existingPhoneNumbers.Count < 2)
                    {
                        updatedPhoneNumbers.Add(new PhoneNumber
                        {
                            Id = Guid.NewGuid(),
                            UserId = userId,
                            Mobile1 = dto.Mobile1
                        });
                    }
                    else
                    {
                        throw new InvalidOperationException("Cannot add more than two phone numbers.");
                    }
                }
                else
                {
                    // Updating an existing phone number
                    var existingPhoneNumber = existingPhoneNumbers.FirstOrDefault(p => p.Id == dto.Id);
                    if (existingPhoneNumber != null)
                    {
                        existingPhoneNumber.Mobile1 = dto.Mobile1;
                        updatedPhoneNumbers.Add(existingPhoneNumber);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Phone number with ID {dto.Id} does not exist.");
                    }
                }
            }

            // Ensure the updated phone numbers count does not exceed 2
            if (updatedPhoneNumbers.Count > 2)
            {
                throw new InvalidOperationException("A user cannot have more than two phone numbers.");
            }

            // Save the changes to the repository
            await _userRepository.UpdatePhoneNumbersAsync(userId, updatedPhoneNumbers);

            // Map updated entities to response DTOs
            return updatedPhoneNumbers.Select(phone => new PhoneNumberResponseDTO
            {
                Id = phone.Id,
                Mobile1 = phone.Mobile1
            }).ToList();
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
                Status = createdCustomer.status,
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
        public async Task<(CustomerResponseDto customer, List<CarCustomerDTO>? rentedCars, string message)> GetCustomerDetailsWithRentalInfoAsync(Guid customerId)
        {
            // Fetch customer details
            var customer = await _userRepository.GetCustomerByIdAsync(customerId);
            if (customer == null)
            {
                return (null, null, "Customer not found.");
            }

            // Fetch rental requests for the customer
            var rentalRequests = await _rentalRequestRepository.GetRentalRequestsByCustomerIdAsync(customerId);

            if (!rentalRequests.Any())
            {
                return (MapUserToDto(customer), null, "Customer details returned. No rental requests found.");
            }

            // Filter approved rental requests
            var approvedRequests = rentalRequests.Where(r => r.Action.Equals("Approved", StringComparison.OrdinalIgnoreCase)).ToList();

            if (!approvedRequests.Any())
            {
                return (MapUserToDto(customer), null, "Customer details returned. No approved rental requests found.");
            }

            // Map approved rentals to car details
            var rentedCars = approvedRequests.Select(r => MapCarToDto(r.Car, r)).ToList();
            return (MapUserToDto(customer), rentedCars, "Customer and approved rental car details returned.");
        }

        private CustomerResponseDto MapUserToDto(User user)
        {
            return new CustomerResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Licence = user.Licence,
                Status = user.status,
                Notes = user.Notes,
                NIC = user.NIC,
                PhoneNumbers = user.PhoneNumbers?.Select(p => new PhoneNumberDTO
                {

                    Mobile1 = p.Mobile1
                }).ToList(),
                Addresses = user.Addresses?.Select(a => new AddressDTO
                {
                    HouseNo = a.HouseNo,
                    Street1 = a.Street1,
                    Street2 = a.Street2,
                    City = a.City,
                    Country = a.Country,
                    ZipCode = a.ZipCode
                }).ToList()
            };
        }

        private CarCustomerDTO MapCarToDto(Car car, RentalRequest rentalRequest)
        {
            return new CarCustomerDTO
            {
                Id = car.Id,
                BrandId = car.BrandId,
                BrandName = car.Brand?.Name ?? "Unknown", // Check for null Brand
                ModelId = car.ModelId,
                ModelName = car.Model?.Name ?? "Unknown", // Check for null Model
                RegNo = car.RegNo ?? "N/A",
                PricePerDay = car.PricePerDay,
                GearType = car.GearType ?? "N/A",
                FuelType = car.FuelType ?? "N/A",
                SeatCount = car.SeatCount,
                Mileage = car.Mileage,
                Images = car.Images?.Select(img => new ImageDTO
                {
                    Id = img.Id,
                    ImagePath = img.ImagePath ?? "No Image"
                }).ToList() ?? new List<ImageDTO>(),
                Status = car.Status ?? "Unavailable",
                StartDate = rentalRequest?.StartDate, // Handle potential null
                EndDate = rentalRequest?.EndDate, // Handle potential null
                Duration = rentalRequest?.Duration ?? 0 // Provide default value if null
            };


        }


    }
}






