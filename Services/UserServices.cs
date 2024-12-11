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
using System.ComponentModel;
using Microsoft.AspNetCore.Identity;
using DriveX_Backend.Migrations;

namespace DriveX_Backend.Services
{
    public class UserServices : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IRentalRequestRepository _rentalRequestRepository;
        private readonly string _jwtSecret = "your-very-secure-key-with-at-least-32-characters";
        private readonly WhatsAppService _whatsAppService;

        public UserServices(IUserRepository userRepository, IConfiguration configuration, IEmailService emailService, IRentalRequestRepository rentalRequestRepository, WhatsAppService whatsAppService)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _emailService = emailService;
            _rentalRequestRepository = rentalRequestRepository;
            _whatsAppService = whatsAppService;
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

            user.Token = CreateJwt(user);
            var newAccessToken = user.Token;
            var newRefreshToken = CreateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.Now.AddDays(1);
            if (string.IsNullOrEmpty(user.Token))
            {
                throw new ApplicationException("Failed to generate access token");
            }

            await _userRepository.UpdateUserRefreshTokenAsync(user);

            return new TokenApiDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task<bool> ChangePasswordAsync(UpdatePasswordDTO updatePasswordDTO)
        {
            var user = await _userRepository.GetCustomerByIdAsync(updatePasswordDTO.Id);
            if (user == null)
            {
                throw new Exception("User not found.");
            };

            var veriyPassword = BCrypt.Net.BCrypt.Verify(updatePasswordDTO.OldPassword, user.Password);
            if (!veriyPassword)
            {
                throw new Exception("Old password is incorrect.");
            }
            if (updatePasswordDTO.NewPassword != updatePasswordDTO.ConfirmPassword)
            {
                throw new Exception("New password and confirmation password do not match.");
            }
            user.Password = PasswordValidator.HashPassword(updatePasswordDTO.NewPassword);

            await _userRepository.UpdateCustomerAsync(user);
            return true;
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
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiry <= DateTime.Now)
            {
                throw new SecurityTokenException("Invalid or expired refresh token.");
            }


            var newAccessToken = CreateJwt(user);
            var newRefreshToken = CreateRefreshToken();

            user.RefreshToken = newRefreshToken;
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
                Addresses = user.Addresses?.Select(a => new AddressResponseDTO
                {
                    Id = a.Id,
                    HouseNo = a.HouseNo,
                    Street1 = a.Street1,
                    Street2 = a.Street2,
                    City = a.City,
                    ZipCode = a.ZipCode,
                    Country = a.Country,
                }).ToList(),
                PhoneNumbers = user.PhoneNumbers?.Select(n => new PhoneNumberResponseDTO
                {
                    Id = n.Id,
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


            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var emailToken = Convert.ToBase64String(tokenBytes);


            user.ForgetPasswordToken = emailToken;
            user.ForgetPasswordTokenExpiry = DateTime.UtcNow.AddMinutes(15);


            await _userRepository.ResetPasswordChange(user);


            string from = _configuration["EmailSettings:From"];
            var emailModel = new EmailModel(email, "Reset Password!",
                ResetEmailBody.ResetPasswordEmailStringBody(email, emailToken));

            _emailService.SendPasswordResetEmail(emailModel);
            string userPhoneNumber = "+94774477065"; // Replace with a dynamic number, e.g., user.PhoneNumber
            _whatsAppService.SendWhatsAppMessage(userPhoneNumber, "approved");

            return emailModel;
        }




        public async Task<User> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            if (resetPasswordDTO == null)
                throw new ArgumentNullException(nameof(resetPasswordDTO));

            var newToken = resetPasswordDTO.EmailToken.Replace(" ", "+");


            var user = await _userRepository.GetUserByEmailAsync(resetPasswordDTO.Email);
            if (user == null)
                throw new Exception("User not found.");


            if (user.ForgetPasswordToken != newToken || user.ForgetPasswordTokenExpiry < DateTime.UtcNow)
                throw new Exception("Invalid or expired reset token.");



            if (resetPasswordDTO.NewPassword != resetPasswordDTO.ConfirmPassword)
                throw new Exception("Passwords do not match.");



            user.Password = PasswordValidator.HashPassword(resetPasswordDTO.NewPassword);


            user.ForgetPasswordToken = null;
            user.ForgetPasswordTokenExpiry = null;


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
                Expires = DateTime.UtcNow.AddMinutes(1),
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
                if (tokenExists)
                {
                    return CreateRefreshToken();
                }
                return refreshToken;
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
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("This is Invalid Token");
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
                        {
                            Id = a.Id,
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
            /*var customerWithSameNIC = await _userRepository.GetUserByNICAsync(updateDTO.NIC);
            if (customerWithSameNIC != null && customerWithSameNIC.Id != id)
            {
                throw new ArgumentException("A customer with this NIC already exists.", nameof(updateDTO.NIC));
            }

            // Check for Email conflict
            var customerWithSameEmail = await _userRepository.GetUserByEmailAsync(updateDTO.Email);
            if (customerWithSameEmail != null && customerWithSameEmail.Id != id)
            {
                throw new ArgumentException("A customer with this email already exists.", nameof(updateDTO.Email));
            }*/

            // Update customer properties
            existingCustomer.FirstName = updateDTO.FirstName;
            existingCustomer.LastName = updateDTO.LastName;
            existingCustomer.Image = updateDTO.Image;
            existingCustomer.NIC = updateDTO.NIC;
            existingCustomer.Licence = updateDTO.Licence;
            existingCustomer.Email = updateDTO.Email;
            existingCustomer.Notes = updateDTO.Notes;
            existingCustomer.status = updateDTO.Status;

            // Update Addresses
            if (updateDTO.Addresses != null && updateDTO.Addresses.Any())
            {
                if (updateDTO.Addresses.Count > 2)
                {
                    throw new InvalidOperationException("A customer cannot have more than two addresses.");
                }

                foreach (var addressDTO in updateDTO.Addresses)
                {
                    var existingAddress = existingCustomer.Addresses.FirstOrDefault(a => a.HouseNo == addressDTO.HouseNo);

                    if (existingAddress != null)
                    {
                        existingAddress.Street1 = addressDTO.Street1;
                        existingAddress.Street2 = addressDTO.Street2;
                        existingAddress.City = addressDTO.City;
                        existingAddress.ZipCode = addressDTO.ZipCode;
                        existingAddress.Country = addressDTO.Country;
                    }
                    else
                    {
                        if (existingCustomer.Addresses.Count >= 2)
                        {
                            throw new InvalidOperationException("Cannot add more than two addresses.");
                        }

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

            // Update PhoneNumbers
            if (updateDTO.PhoneNumbers != null && updateDTO.PhoneNumbers.Any())
            {
                if (updateDTO.PhoneNumbers.Count > 2)
                {
                    throw new InvalidOperationException("A customer cannot have more than two phone numbers.");
                }

                foreach (var phoneDTO in updateDTO.PhoneNumbers)
                {
                    if (!PhoneNumberValidator.IsValidPhoneNumber(phoneDTO.Mobile1))
                    {
                        throw new ArgumentException($"Invalid phone number format: {phoneDTO.Mobile1}");
                    }

                    var existingPhoneNumber = existingCustomer.PhoneNumbers.FirstOrDefault(p => p.Mobile1 == phoneDTO.Mobile1);

                    if (existingPhoneNumber != null)
                    {
                        existingPhoneNumber.Mobile1 = phoneDTO.Mobile1;
                    }
                    else
                    {
                        if (existingCustomer.PhoneNumbers.Count >= 2)
                        {
                            throw new InvalidOperationException("Cannot add more than two phone numbers.");
                        }

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
                Status = existingCustomer.status,
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
                    Mobile1 = p.Mobile1
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
                if (!PhoneNumberValidator.IsValidPhoneNumber(dto.Mobile1))
                {
                    throw new ArgumentException($"Invalid phone number format: {dto.Mobile1}");
                }
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

            // Validate NIC
            if (!NICValidator.IsValidNIC(dashboardRequestCustomerDTO.NIC))
            {
                throw new ArgumentException("Invalid NIC format.", nameof(dashboardRequestCustomerDTO.NIC));
            }

            // Validate Email
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

            // Validate and limit phone numbers
            var validPhoneNumbers = dashboardRequestCustomerDTO.PhoneNumbers?
                .Where(p => PhoneNumberValidator.IsValidPhoneNumber(p.Mobile1))
                .Take(2)
                .Select(p => new PhoneNumber { Mobile1 = p.Mobile1 })
                .ToList();

            if (validPhoneNumbers == null || validPhoneNumbers.Count != dashboardRequestCustomerDTO.PhoneNumbers?.Count)
            {
                throw new ArgumentException("One or more phone numbers are invalid.", nameof(dashboardRequestCustomerDTO.PhoneNumbers));
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
                status = dashboardRequestCustomerDTO.status,
                Password = PasswordValidator.HashPassword(dashboardRequestCustomerDTO.Password),
                Addresses = dashboardRequestCustomerDTO.Addresses?.Take(2).Select(a => new Address
                {
                    HouseNo = a.HouseNo,
                    Street1 = a.Street1,
                    Street2 = a.Street2,
                    City = a.City,
                    ZipCode = a.ZipCode,
                    Country = a.Country
                }).ToList(),
                PhoneNumbers = validPhoneNumbers
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
                OngoingRevenue = user.OngoingRevenue,
                TotalRevenue = user.TotalRevenue,
                PhoneNumbers = user.PhoneNumbers?.Select(p => new PhoneNumberResponseDTO
                {

                    Mobile1 = p.Mobile1
                }).ToList(),
                Addresses = user.Addresses?.Select(a => new AddressResponseDTO
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

        public async Task<List<UpdateManagerDTO>> GetAllManagersAsync()
        {
            try
            {
                var users = await _userRepository.GetAllManagersAsync();

                if (users == null || !users.Any())
                {
                    return new List<UpdateManagerDTO>();
                }

                return users.Select(u => new UpdateManagerDTO
                {
                    Id = u.Id,
                    FirstName = u.FirstName ?? "N/A",  // Handle potential null values
                    LastName = u.LastName ?? "N/A",    // Handle potential null values
                    Image = u.Image ?? string.Empty,   // Handle potential null image paths
                    NIC = u.NIC ?? "N/A",              // Handle potential null values

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
                        {
                            Id = a.Id,
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

        public async Task<UpdateManagerDTO> UpdateManagerAsync(Guid id, ManagerDTO updateDTO)
        {
            if (updateDTO == null)
            {
                throw new ArgumentNullException(nameof(updateDTO), "Manager data cannot be null.");
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

            // Retrieve the existing manager
            var existingManager = await _userRepository.GetCustomerByIdAsync(id);
            if (existingManager == null || existingManager.Role != Role.Manager)
            {
                throw new KeyNotFoundException("Manager not found.");
            }

            // Update manager properties
            existingManager.FirstName = updateDTO.FirstName;
            existingManager.LastName = updateDTO.LastName;
            existingManager.Image = updateDTO.Image;
            existingManager.NIC = updateDTO.NIC;

            existingManager.Email = updateDTO.Email;
            existingManager.Notes = updateDTO.Notes;


            // Update Addresses
            if (updateDTO.Addresses != null && updateDTO.Addresses.Any())
            {
                if (updateDTO.Addresses.Count > 2)
                {
                    throw new InvalidOperationException("A manager cannot have more than two addresses.");
                }

                foreach (var addressDTO in updateDTO.Addresses)
                {
                    var existingAddress = existingManager.Addresses.FirstOrDefault(a => a.HouseNo == addressDTO.HouseNo);

                    if (existingAddress != null)
                    {
                        existingAddress.Street1 = addressDTO.Street1;
                        existingAddress.Street2 = addressDTO.Street2;
                        existingAddress.City = addressDTO.City;
                        existingAddress.ZipCode = addressDTO.ZipCode;
                        existingAddress.Country = addressDTO.Country;
                    }
                    else
                    {
                        if (existingManager.Addresses.Count >= 2)
                        {
                            throw new InvalidOperationException("Cannot add more than two addresses.");
                        }

                        existingManager.Addresses.Add(new Address
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

            // Update PhoneNumbers
            if (updateDTO.PhoneNumbers != null && updateDTO.PhoneNumbers.Any())
            {
                if (updateDTO.PhoneNumbers.Count > 2)
                {
                    throw new InvalidOperationException("A manager cannot have more than two phone numbers.");
                }

                foreach (var phoneDTO in updateDTO.PhoneNumbers)
                {
                    if (!PhoneNumberValidator.IsValidPhoneNumber(phoneDTO.Mobile1))
                    {
                        throw new ArgumentException($"Invalid phone number format: {phoneDTO.Mobile1}");
                    }

                    var existingPhoneNumber = existingManager.PhoneNumbers.FirstOrDefault(p => p.Mobile1 == phoneDTO.Mobile1);

                    if (existingPhoneNumber != null)
                    {
                        existingPhoneNumber.Mobile1 = phoneDTO.Mobile1;
                    }
                    else
                    {
                        if (existingManager.PhoneNumbers.Count >= 2)
                        {
                            throw new InvalidOperationException("Cannot add more than two phone numbers.");
                        }

                        existingManager.PhoneNumbers.Add(new PhoneNumber
                        {
                            Mobile1 = phoneDTO.Mobile1
                        });
                    }
                }
            }

            // Save changes to the database
            await _userRepository.SaveAsync();

            // Return the updated manager response DTO
            return new UpdateManagerDTO
            {

                FirstName = existingManager.FirstName,
                LastName = existingManager.LastName,
                Image = existingManager.Image,
                NIC = existingManager.NIC,

                Email = existingManager.Email,
                Notes = existingManager.Notes,
                Addresses = existingManager.Addresses.Select(a => new AddressResponseDTO
                {
                    Id = a.Id,
                    HouseNo = a.HouseNo,
                    Street1 = a.Street1,
                    Street2 = a.Street2,
                    City = a.City,
                    ZipCode = a.ZipCode,
                    Country = a.Country
                }).ToList(),
                PhoneNumbers = existingManager.PhoneNumbers.Select(p => new PhoneNumberResponseDTO
                {
                    Id = p.Id,
                    Mobile1 = p.Mobile1
                }).ToList()
            };
        }
        public async Task<ManagerDTO> GetManagerByIdAsync(Guid id)
        {
            // Retrieve the manager entity from the repository
            var manager = await _userRepository.GetManagerByIdAsync(id);
            if (manager == null)
            {
                throw new KeyNotFoundException("Manager not found.");
            }

            // Map the manager entity to ManagerDTO
            var managerDTO = new ManagerDTO
            {
                Image = manager.Image,
                FirstName = manager.FirstName,
                LastName = manager.LastName,
                NIC = manager.NIC,
                Email = manager.Email,
                Notes = manager.Notes,
                Addresses = manager.Addresses.Select(a => new AddressDTO
                {
                    HouseNo = a.HouseNo,
                    Street1 = a.Street1,
                    Street2 = a.Street2,
                    City = a.City,
                    ZipCode = a.ZipCode,
                    Country = a.Country
                }).ToList(),
                PhoneNumbers = manager.PhoneNumbers.Select(p => new PhoneNumberDTO
                {
                    Mobile1 = p.Mobile1
                }).ToList()
            };

            return managerDTO;
        }
        public async Task<DashboardAllManagerDTO> AddManagerDashboard(DashboardRequestManagerDTO dashboardRequestManagerDTO)
        {
            if (dashboardRequestManagerDTO == null)
            {
                throw new ArgumentNullException(nameof(dashboardRequestManagerDTO), "Manager data cannot be null");
            }

            // Validate NIC
            if (!NICValidator.IsValidNIC(dashboardRequestManagerDTO.NIC))
            {
                throw new ArgumentException("Invalid NIC format.", nameof(dashboardRequestManagerDTO.NIC));
            }

            // Validate Email
            if (!EmailValidator.IsValidEmail(dashboardRequestManagerDTO.Email))
            {
                throw new ArgumentException("Invalid email format.", nameof(dashboardRequestManagerDTO.Email));
            }

            // Check for existing managers
            var existingManager = await _userRepository.GetUserByNICAndRoleAsync(dashboardRequestManagerDTO.NIC, Role.Manager);
            if (existingManager != null)
            {
                throw new ArgumentException("A manager with this NIC already exists.", nameof(dashboardRequestManagerDTO.NIC));
            }

            // Validate and limit phone numbers
            var validPhoneNumbers = dashboardRequestManagerDTO.PhoneNumbers?
                .Where(p => PhoneNumberValidator.IsValidPhoneNumber(p.Mobile1))
                .Take(2)
                .Select(p => new PhoneNumber { Mobile1 = p.Mobile1 })
                .ToList();

            if (validPhoneNumbers == null || validPhoneNumbers.Count != dashboardRequestManagerDTO.PhoneNumbers?.Count)
            {
                throw new ArgumentException("One or more phone numbers are invalid.", nameof(dashboardRequestManagerDTO.PhoneNumbers));
            }

            // Map the DTO to the entity
            var newManager = new User
            {
                FirstName = dashboardRequestManagerDTO.FirstName,
                LastName = dashboardRequestManagerDTO.LastName,
                Image = dashboardRequestManagerDTO.Image,
                NIC = dashboardRequestManagerDTO.NIC,
                Email = dashboardRequestManagerDTO.Email,
                Notes = dashboardRequestManagerDTO.Notes,
                Role = Role.Manager, // Assign manager-specific role
                Password = PasswordValidator.HashPassword(dashboardRequestManagerDTO.Password),
                Addresses = dashboardRequestManagerDTO.Addresses?.Take(2).Select(a => new Address
                {
                    HouseNo = a.HouseNo,
                    Street1 = a.Street1,
                    Street2 = a.Street2,
                    City = a.City,
                    ZipCode = a.ZipCode,
                    Country = a.Country
                }).ToList(),
                PhoneNumbers = validPhoneNumbers
            };

            // Save the manager to the repository
            var createdManager = await _userRepository.AddManagerDashboard(newManager);

            // Map the created manager back to the DTO
            return new DashboardAllManagerDTO
            {
                Id = createdManager.Id,
                FirstName = createdManager.FirstName,
                LastName = createdManager.LastName,
                Email = createdManager.Email,
                NIC = createdManager.NIC,
                Role = createdManager.Role.ToString(),
                Notes = createdManager.Notes,
                Status = createdManager.status,
                Addresses = createdManager.Addresses?.Select(a => new AddressResponseDTO
                {
                    Id = Guid.NewGuid(),
                    HouseNo = a.HouseNo,
                    Street1 = a.Street1,
                    Street2 = a.Street2,
                    City = a.City,
                    ZipCode = a.ZipCode,
                    Country = a.Country
                }).ToList(),
                PhoneNumbers = createdManager.PhoneNumbers?.Select(p => new PhoneNumberResponseDTO
                {
                    Id = Guid.NewGuid(),
                    Mobile1 = p.Mobile1,
                }).ToList()
            };
        }
        public async Task<ProfileCustomerResponse> UpdateUserProfileAsync(Guid userId, ProfileCustomerRequest profileCustomerRequest)
        {
            if (string.IsNullOrWhiteSpace(profileCustomerRequest.Licence))
                throw new ArgumentException("Licence is required");

            var user = await _userRepository.GetCustomerByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            // Map ProfileCustomerRequest DTO to User entity
            if (!string.IsNullOrWhiteSpace(profileCustomerRequest.Image))
            {
                user.Image = profileCustomerRequest.Image;
            }
            user.FirstName = profileCustomerRequest.FirstName;
            user.LastName = profileCustomerRequest.LastName;
            user.Licence = profileCustomerRequest.Licence;

            await _userRepository.UpdateUserAsync(user);

            // Map User entity back to ProfileCustomerResponse DTO
            return new ProfileCustomerResponse
            {
                Id = user.Id,
                Image = user.Image,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Licence = user.Licence
            };

        }
        public async Task<List<AddressResponseDTO>> GetCustomerAddressesAsync(Guid customerId)
        {
            var addresses = await _userRepository.GetAddressesByCustomerIdAsync(customerId);

            // Map entities to DTOs
            var addressResponseDTOs = addresses.Select(a => new AddressResponseDTO
            {
                Id = a.Id,
                HouseNo = a.HouseNo,
                Street1 = a.Street1,
                Street2 = a.Street2,
                City = a.City,
                ZipCode = a.ZipCode,
                Country = a.Country
            }).ToList();

            return addressResponseDTOs;
        }

        // Add a new address for a customer (up to 2 addresses)
        public async Task<AddressResponseDTO> AddCustomerAddressAsync(Guid customerId, AddressDTO addressDTO)
        {
            // Get existing addresses
            var existingAddresses = await _userRepository.GetAddressesByCustomerIdAsync(customerId);

            if (existingAddresses.Count >= 2)
            {
                throw new InvalidOperationException("You can only have up to 2 addresses.");
            }

            // Create the new Address entity from DTO
            var newAddress = new Address
            {
                Id = Guid.NewGuid(),
                UserId = customerId,
                HouseNo = addressDTO.HouseNo,
                Street1 = addressDTO.Street1,
                Street2 = addressDTO.Street2,
                City = addressDTO.City,
                ZipCode = addressDTO.ZipCode,
                Country = addressDTO.Country,

            };

            // Save the address
            var savedAddress = await _userRepository.AddAddressAsync(newAddress);

            // Map to response DTO
            var addressResponseDTO = new AddressResponseDTO
            {
                Id = savedAddress.Id,
                HouseNo = savedAddress.HouseNo,
                Street1 = savedAddress.Street1,
                Street2 = savedAddress.Street2,
                City = savedAddress.City,
                ZipCode = savedAddress.ZipCode,
                Country = savedAddress.Country
            };

            return addressResponseDTO;
        }
        public async Task<AddressResponseDTO> UpdateAddressAsync(Guid addressId, AddressDTO addressDto)
        {
            // 1. Get the existing address from the database
            var existingAddress = await _userRepository.GetAddressByIdAsync(addressId);

            if (existingAddress == null)
                throw new KeyNotFoundException("Address not found.");

            // 2. Update the existing address with new values from AddressDTO
            existingAddress.HouseNo = addressDto.HouseNo;
            existingAddress.Street1 = addressDto.Street1;
            existingAddress.Street2 = addressDto.Street2;
            existingAddress.City = addressDto.City;
            existingAddress.ZipCode = addressDto.ZipCode;
            existingAddress.Country = addressDto.Country;

            // 3. Save the updated address to the database
            await _userRepository.UpdateAddressAsync(existingAddress);

            // 4. Manually map the updated Address entity to AddressResponseDTO
            var responseDto = new AddressResponseDTO
            {
                Id = existingAddress.Id,
                HouseNo = existingAddress.HouseNo,
                Street1 = existingAddress.Street1,
                Street2 = existingAddress.Street2,
                City = existingAddress.City,
                ZipCode = existingAddress.ZipCode,
                Country = existingAddress.Country
            };

            return responseDto;
        }
        public async Task<bool> DeleteAddressByIdAsync(Guid addressId)
        {
            // Step 1: Get the address by its ID
            var address = await _userRepository.GetAddressByIdAsync(addressId);

            if (address == null)
            {
                throw new KeyNotFoundException("Address not found.");
            }

            // Step 2: Delete the address
            return await _userRepository.DeleteAddressAsync(address);
        }
        public async Task<PhoneNumberResponseDTO> AddPhoneNumberAsync(Guid customerId, PhoneNumberDTO phoneNumberDTO)
        {
            // Validate the phone number using PhoneNumberValidator
            if (!PhoneNumberValidator.IsValidPhoneNumber(phoneNumberDTO.Mobile1))
            {
                throw new ArgumentException("The phone number is not valid.");
            }

            // Get existing phone numbers for the customer
            var existingPhoneNumbers = await _userRepository.GetPhoneNumbersByCustomerIdAsync(customerId);

            // Check if the customer already has 2 phone numbers
            if (existingPhoneNumbers.Count >= 2)
            {
                throw new InvalidOperationException("You can only have up to 2 phone numbers.");
            }

            // Create the new PhoneNumber entity from DTO
            var newPhoneNumber = new PhoneNumber
            {
                Id = Guid.NewGuid(),
                UserId = customerId,
                Mobile1 = phoneNumberDTO.Mobile1
            };

            // Save the new phone number to the repository
            var savedPhoneNumber = await _userRepository.AddPhoneNumberAsync(newPhoneNumber);

            // Map to response DTO
            var phoneNumberResponseDTO = new PhoneNumberResponseDTO
            {
                Id = savedPhoneNumber.Id,
                Mobile1 = savedPhoneNumber.Mobile1
            };

            return phoneNumberResponseDTO;
        }


        // **Update Phone Number**
        public async Task<PhoneNumberResponseDTO> UpdatePhoneNumberAsync(Guid phoneNumberId, PhoneNumberDTO phoneNumberDTO)
        {
            // Retrieve the phone number by its ID
            var phoneNumber = await _userRepository.GetPhoneNumberByIdAsync(phoneNumberId);
            if (phoneNumber == null)
                throw new Exception("Phone number not found.");

            // Validate the phone number using PhoneNumberValidator
            if (!string.IsNullOrWhiteSpace(phoneNumberDTO.Mobile1) && !PhoneNumberValidator.IsValidPhoneNumber(phoneNumberDTO.Mobile1))
            {
                throw new ArgumentException("The phone number is not valid.");
            }

            // Update the phone number if it is not empty and valid
            if (!string.IsNullOrWhiteSpace(phoneNumberDTO.Mobile1))
            {
                phoneNumber.Mobile1 = phoneNumberDTO.Mobile1;
            }

            // Save the updated phone number to the repository
            var updatedPhoneNumber = await _userRepository.UpdatePhoneNumberAsync(phoneNumber);

            // Return the updated phone number in the response DTO
            return new PhoneNumberResponseDTO
            {
                Id = updatedPhoneNumber.Id,
                Mobile1 = updatedPhoneNumber.Mobile1
            };
        }

        // **Delete Phone Number**
        public async Task<bool> DeletePhoneNumberAsync(Guid phoneNumberId)
        {
            var isDeleted = await _userRepository.DeletePhoneNumberAsync(phoneNumberId);
            if (!isDeleted) throw new Exception("Phone number not found or already deleted.");
            return isDeleted;
        }
    }
}

    
















