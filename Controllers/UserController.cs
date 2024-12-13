using DriveX_Backend.DB;
using DriveX_Backend.Entities.Users;
using DriveX_Backend.Entities.Users.Models;
using DriveX_Backend.Entities.Users.UserDTO;
using DriveX_Backend.Helpers;
using DriveX_Backend.IServices;
using DriveX_Backend.Services;
using DriveX_Backend.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Web;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace DriveX_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly AppDbContext _appDbContext;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public UserController(IUserService userService, AppDbContext appDbContext, IEmailService emailService, IConfiguration configuration)
        {
            _userService = userService;
            _appDbContext = appDbContext;
            _emailService = emailService;
            _configuration = configuration;


        }

        [HttpGet("customer/{id}")]
        public async Task<IActionResult> GetCustomerById(Guid id)
        {
            try
            {
                var customer = await _userService.GetCustomerById(id);
                if (customer == null)
                {
                    return NotFound(new { Message = "Customer not found" });
                }

                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred", Details = ex.Message });
            }
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest signInRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var tokenResponse = await _userService.AuthenticateUserAsync(signInRequest);
                return Ok(tokenResponse);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ApplicationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An unexpected error occurred.",
                    details = ex.Message
                });
            }
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenApiDTO tokenApiDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newTokens = await _userService.Refresh(tokenApiDTO);
                return Ok(newTokens);
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An unexpected error occurred.",
                    details = ex.Message
                });
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterCustomer([FromBody] SignupRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var customerResponse = await _userService.CustomerRegister(request);
                return CreatedAtAction(nameof(GetCustomerById), new { id = customerResponse.Id }, customerResponse);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Unexpected error: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred during the registration process. Please try again later." });
            }


        }


        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] UpdatePasswordDTO updatePasswordDTO)
        {
            try
            {
                await _userService.ChangePasswordAsync(updatePasswordDTO);
                return Ok(new { message = "Password updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpGet("all")]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        [HttpGet("all-customers-list")]
        public async Task<IActionResult> DashboardAllCustomersAsync()
        {
            try
            {
                var customers = await _userService.DashboardAllCustomersAsync();

                if (customers == null || !customers.Any())
                {
                    return NotFound(new { message = "No customers found." });
                }

                return Ok(customers);
            }
            catch (Exception ex)
            {
                // Log the exception here if needed
                return StatusCode(500, new { message = "An error occurred while retrieving customers.", error = ex.Message });
            }
        }


        [HttpPost("send-reset-email")]
        public async Task<IActionResult> SendResetEmailAsync([FromBody] ResetEmailRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Email cannot be empty."
                });
            }

            try
            {
                var emailModel = await _userService.SendResetEmail(request.Email);
                if (emailModel == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "Email doesn't exist."
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Reset password email sent successfully.",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while processing your request. Please try again later."
                });
            }
        }







        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            if (resetPasswordDTO == null)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Invalid request data."
                });
            }

            try
            {
                var user = await _userService.ResetPassword(resetPasswordDTO);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Password has been reset successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] UpdateUserDTO updateDTO)
        {
            if (updateDTO == null)
            {
                return BadRequest("User data is required.");
            }

            try
            {
                var updatedUser = await _userService.UpdateCustomerAsync(id, updateDTO);
                return Ok(updatedUser);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("customersAddresses")]
        public async Task<IActionResult> UpdateAddresses(Guid userId, List<AddressResponseDTO> addressDTOs)
        {
            try
            {
                if (addressDTOs == null || !addressDTOs.Any())
                {
                    return BadRequest("Address list cannot be empty");
                }
                var updatedAddresses = await _userService.UpdateAddressAsync(userId, addressDTOs);
                return Ok(updatedAddresses);

            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An unexpected error occurred.", Details = ex.Message });
            }

        }



        [HttpPost("UpdatePhoneNumbers")]
        public async Task<IActionResult> UpdatePhoneNumbers(Guid userId, [FromBody] List<PhoneNumberResponseDTO> phoneNumberDTOs)
        {
            try
            {
                if (phoneNumberDTOs == null || !phoneNumberDTOs.Any())
                {
                    return BadRequest("Phone number list cannot be empty.");
                }

                var updatedPhoneNumbers = await _userService.UpdatePhoneNumberAsync(userId, phoneNumberDTOs);
                return Ok(updatedPhoneNumbers);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An unexpected error occurred.", Details = ex.Message });
            }
        }



        [HttpPost("add-customer-dashboard")]
        public async Task<IActionResult> AddCustomerDashboard([FromBody] DashboardRequestCustomerDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var customerResponse = await _userService.AddCustomerDashboard(request);
                return CreatedAtAction(nameof(GetCustomerById), new { id = customerResponse.Id }, customerResponse);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }


        }
        [HttpDelete("deleteCustomer/{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            try
            {
                var success = await _userService.DeleteCustomerAsync(id);
                if (success)
                {
                    return NoContent(); // Successfully deleted
                }

                return NotFound("Customer not found.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal server error
            }
        }
        [HttpGet("GetCustomerWithRentalInfo/{customerId}")]
        public async Task<IActionResult> GetCustomerWithRentalInfo(Guid customerId)
        {
            var result = await _userService.GetCustomerDetailsWithRentalInfoAsync(customerId);

            if (result.customer == null)
                return NotFound(new { message = result.message });

            return Ok(new
            {
                Customer = result.customer,
                RentedCars = result.rentedCars,
                Message = result.message
            });
        }
        [Authorize]
        [HttpGet("all-managers-list")]
        public async Task<IActionResult> GetAllManagersAsync()
        {
            try
            {
                var managers = await _userService.GetAllManagersAsync();

                if (managers == null || !managers.Any())
                {
                    return NotFound(new { message = "No managers found." });
                }

                return Ok(managers);
            }
            catch (Exception ex)
            {
                // Log the exception here if needed
                return StatusCode(500, new { message = "An error occurred while retrieving managers.", error = ex.Message });
            }
        }


        [HttpPut("update-manager/{id}")]
        public async Task<IActionResult> UpdateManager(Guid id, [FromBody] ManagerDTO updateDTO)
        {
            if (updateDTO == null)
            {
                return BadRequest("Manager data is required.");
            }

            try
            {
                var updatedManager = await _userService.UpdateManagerAsync(id, updateDTO);
                return Ok(updatedManager);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("manger/{id}")]
        public async Task<IActionResult> GetManagerById(Guid id)
        {
            try
            {
                var manager = await _userService.GetManagerByIdAsync(id);
                return Ok(manager);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
        [HttpPost("add-manager-dashboard")]
        public async Task<IActionResult> AddManagerDashboard([FromBody] DashboardRequestManagerDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var managerResponse = await _userService.AddManagerDashboard(request);
                return CreatedAtAction(nameof(GetManagerById), new { id = managerResponse.Id }, managerResponse);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpPut("{userId}/update-profile")]
        public async Task<IActionResult> UpdateUserProfile(Guid userId, [FromBody] ProfileCustomerRequest profileCustomer)
        {
            try
            {
                var response = await _userService.UpdateUserProfileAsync(userId, profileCustomer);
                return Ok(response);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User not found");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("address/{customerId:guid}")]
        public async Task<IActionResult> GetCustomerAddresses(Guid customerId)
        {
            var addresses = await _userService.GetCustomerAddressesAsync(customerId);
            return Ok(addresses);
        }

        /// <summary>
        /// Add a new address for a specific customer
        /// </summary>
        [HttpPost("address/{customerId:guid}")]
        public async Task<IActionResult> AddCustomerAddress(Guid customerId, [FromBody] AddressDTO addressDTO)
        {
            try
            {
                var address = await _userService.AddCustomerAddressAsync(customerId, addressDTO);
                return CreatedAtAction(nameof(GetCustomerAddresses), new { customerId = customerId }, address);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("{id:guid}/updateAddress")]
        public async Task<IActionResult> UpdateAddress([FromRoute] Guid id, [FromBody] AddressDTO addressDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedAddress = await _userService.UpdateAddressAsync(id, addressDto);
                return Ok(updatedAddress);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the address.", error = ex.Message });
            }
        }
        [HttpDelete("{addressId}/deleteAddress")]
        public async Task<IActionResult> DeleteAddress(Guid addressId)
        {
            try
            {
                var isDeleted = await _userService.DeleteAddressByIdAsync(addressId);

                if (!isDeleted)
                {
                    return NotFound(new { message = "Address not found or could not be deleted." });
                }

                return Ok(new { message = "Address deleted successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the address.", details = ex.Message });
            }
        }
        [HttpPost("{userId}/add-phone-number")]
        public async Task<IActionResult> AddPhoneNumber(Guid userId, [FromBody] PhoneNumberDTO phoneNumberDTO)
        {
            try
            {
                var result = await _userService.AddPhoneNumberAsync(userId, phoneNumberDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // **Update Phone Number**
        [HttpPut("{phoneNumberId}/update-phone-number")]
        public async Task<IActionResult> UpdatePhoneNumber(Guid phoneNumberId, [FromBody] PhoneNumberDTO phoneNumberDTO)
        {
            try
            {
                var result = await _userService.UpdatePhoneNumberAsync(phoneNumberId, phoneNumberDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // **Delete Phone Number**
        [HttpDelete("{phoneNumberId}/delete-phone-number")]
        public async Task<IActionResult> DeletePhoneNumber(Guid phoneNumberId)
        {
            try
            {
                await _userService.DeletePhoneNumberAsync(phoneNumberId);
                return Ok(new { message = "Phone Number deleted successfully." });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        [HttpDelete("delete-manager/{id}")]
        public async Task<IActionResult> DeleteManager(Guid id)
        {
            try
            {
                await _userService.DeleteManagerAsync(id);
                return Ok(new { message = "Manager deleted successfully." });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}

    



