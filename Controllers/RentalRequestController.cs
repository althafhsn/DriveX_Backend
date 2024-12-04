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

        [HttpGet("ongoingRentals")]
        public async Task<IActionResult> GetAllOngoinRentals()
        {
            var ongoing = await _service.GetAllOngoingRentals();
            return Ok(ongoing);
        }

        [HttpGet("allRented")]
        public async Task<IActionResult> GetAllRented()
        {
            var rented = await _service.GetAllRented();
            return Ok(rented);
        }


        [HttpGet("recentRentalRequest")]
        public async Task<IActionResult> GetRecentRentalRequests()
        {
            var recent = await _service.GetRecentRentalRequests();
            return Ok(recent);
        }

        [HttpGet("getAllRentalRequests")]
        public async Task<IActionResult> GetAllRentalRequests()
        {
            var rentalRequests = await _service.GetAllRentalRequestsAsync();
            return Ok(rentalRequests);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetRentalRequestsByCustomerId(Guid customerId)
        {
            try
            {
                var rentalRequests = await _service.GetRentalRequestsByCustomerIdAsync(customerId);

                if (!rentalRequests.Any())
                    return NotFound(new { Message = "No rental requests found for this customer." });

                return Ok(rentalRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing the request.", Error = ex.Message });
            }
        }


    }
}
