using DriveX_Backend.DB;
using DriveX_Backend.Entities.Users;
using DriveX_Backend.Entities.Users.Models;
using DriveX_Backend.Entities.Users.UserDTO;
using DriveX_Backend.Helpers;
using DriveX_Backend.IServices;
using DriveX_Backend.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Web;


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

        public UserController(IUserService userService , AppDbContext appDbContext , IEmailService emailService, IConfiguration configuration)
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



        [HttpPost("send-reset-email/{email}")]


        //public async Task<IActionResult> SendEmail(string email)
        //{
        //    // Decode email if needed
        //    email = Uri.UnescapeDataString(email);

        //    // Check if user exists
        //    var user = await _appDbContext.Users.FirstOrDefaultAsync(a => a.Email == email);
        //    if (user is null)
        //    {
        //        return NotFound(new
        //        {
        //            StatusCode = 404,
        //            Message = "Email Doesn't Exist"
        //        });
        //    }

        //    // Generate token
        //    var tokenBytes = RandomNumberGenerator.GetBytes(64);
        //    var emailToken = WebEncoders.Base64UrlEncode(tokenBytes); // URL-safe token
        //    user.ForgetPasswordToken = emailToken;
        //    user.ForgetPasswordTokenExpiry = DateTime.Now.AddMinutes(15);

        //    // Create email model
        //    string from = _configuration["EmailSettings:From"];
        //    var emailModel = new EmailModel(email, "Reset Password!", ResetEmailBody.ResetPasswordEmailStringBody(email, emailToken));

        //    // Send email
        //    try
        //    {
        //        _emailService.SendPasswordResetEmail(emailModel);
        //    }
        //    catch (Exception ex)
        //    {

        //        return StatusCode(500, new
        //        {
        //            StatusCode = 500,
        //            Message = "Internal Server Error. Unable to send email."
        //        });
        //    }

        //    // Save changes
        //    _appDbContext.Entry(user).State = EntityState.Modified;
        //    await _appDbContext.SaveChangesAsync();

        //    return Ok(new
        //    {
        //        StatusCode = 200,
        //        Message = "Email Sent Successfully!!"
        //    });
        //}


        public async Task<IActionResult> SendResetEmailAsync(string email)
        {
            // Decode the email address
            string decodedEmail = HttpUtility.UrlDecode(email);
            Console.WriteLine(decodedEmail);

            if (string.IsNullOrWhiteSpace(decodedEmail))
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Email cannot be empty."
                });
            }

            try
            {
                var emailModel = await _userService.SendResetEmail(decodedEmail);
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



    }
}
