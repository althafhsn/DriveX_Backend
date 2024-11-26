using DriveX_Backend.Entities.RentalRequest;
using DriveX_Backend.Entities.RentalRequest.Models;
using DriveX_Backend.IServices;
using DriveX_Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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
        public async Task<IActionResult> AddRentalRequest([FromBody] AddRentalRequestDTO requestDTO)
        {
            try
            {
                var response = await _service.AddRentalRequestAsync(requestDTO);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Failed to add rental request", Error = ex.Message });
            }
        }

        [HttpPut("{id:guid}/action")]
        public async Task<IActionResult> UpdateRentalAction(Guid id, [FromBody] UpdateRentalRequestDTO updateDto)
        {
            if (updateDto == null || string.IsNullOrEmpty(updateDto.Action))
            {
                return BadRequest("Action cannot be null or empty.");
            }

            try
            {
                await _service.UpdateRentalActionAsync(id, updateDto.Action);
                return Ok(new { Message = "Action Updated Successfully." });
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

        [HttpPut("{id:guid}/status")]
        public async Task<IActionResult> UpdateRentalStatus(Guid id, [FromBody] UpdateStatusDTO updateStatusDTO)
        {
            // Validate the incoming request
            if (updateStatusDTO == null || string.IsNullOrEmpty(updateStatusDTO.Status))
            {
                return BadRequest("Status cannot be null or empty.");
            }

            try
            {
                // Call service method to update the status
                await _service.UpdateRentalStatusAsync(id, updateStatusDTO.Status);
                return Ok(new { Message = "Status Updated Successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message); // If the rental request with the given id was not found
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}"); // Handle other unexpected errors
            }
        }

        //[HttpGet("GetAllRentalRequests")]
        //public async Task<IActionResult> GetAllRentalRequestsAsync()
        //{
        //    try
        //    {
        //        // Get all rental requests using the service
        //        var rentalRequests = await _service.GetAllRentalRequestsAsync();

        //        // Check if there are any rental requests
        //        if (rentalRequests == null || !rentalRequests.Any())
        //        {
        //            return NoContent();  // Return 204 No Content if there are no rental requests
        //        }

        //        // Return the rental requests as an OK response
        //        return Ok(rentalRequests);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Return 500 Internal Server Error if an exception occurs
        //        return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
        //    }
        //}

        [HttpGet]
        [Route("getAllRentalRequests")]
        public async Task<IActionResult> GetAllRentalRequests()
        {
            var rentalRequests = await _service.GetAllRentalRequestsAsync();
            return Ok(rentalRequests);
        }



    }
}
