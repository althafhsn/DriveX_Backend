using DriveX_Backend.Entities.Users;
using DriveX_Backend.Entities.Users.Models;
using DriveX_Backend.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DriveX_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("customer/{id}")]
        public async Task<IActionResult> GetCustomerById(Guid id)
        {

            try
            {
                var customer = await _userService.GetCustomerById(id);
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }

        }
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] SignInRequest signInRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var authenticatedUser = await _userService.AuthenticateUserAsync(signInRequest);

                return Ok(authenticatedUser);
            }
            catch (UnauthorizedAccessException ex)
            {

                return Unauthorized(new { Message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {

                return BadRequest(new { Message = ex.Message });
            }
            catch (ApplicationException ex)
            {

                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { Message = "An internal server error occurred", Details = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterCustomer([FromBody] SignupRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var customerResponse = await _userService.CustomerRegister(request);

                return CreatedAtAction(nameof(GetCustomerById), new { id = customerResponse.Id.ToString() }, customerResponse);
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

                return StatusCode(500, new { Message = "An internal server error occurred", Details = ex.Message });
            }
        }

    }
}
