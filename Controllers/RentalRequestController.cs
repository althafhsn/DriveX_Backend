using DriveX_Backend.Entities.RentalRequest.Models;
using DriveX_Backend.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DriveX_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalRequestController : ControllerBase
    {
        private readonly IRentalRequestService _service;
        public RentalRequestController(IRentalRequestService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<AddRentalResponseDTO>> AddRental([FromBody] AddRentalRequestDTO requestDTO)
        {
            try
            {
                var result = await _service.AddRentalAsync(requestDTO);
                return CreatedAtAction(nameof(AddRental), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                // Handle specific exceptions with meaningful messages
                return BadRequest(new { error = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                // Database-related errors
                return StatusCode(500, new { error = "A database error occurred.", details = ex.InnerException?.Message ?? ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception for debugging (use a logging framework in production)
                Console.WriteLine($"Unexpected error: {ex}");

                // Return a generic error response
                return StatusCode(500, new { error = "An unexpected error occurred." });
            }
        }

    }

}
