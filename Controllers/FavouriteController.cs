using DriveX_Backend.Entities.Users.FavouriteDTO;
using DriveX_Backend.IServices;
using DriveX_Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DriveX_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavouriteController : ControllerBase
    {
        private readonly IFavouriteService _favouriteService;
        public FavouriteController(IFavouriteService favouriteService)
        {
            _favouriteService = favouriteService;
        }

        [HttpPost("add-to-favorites")]
        public async Task<IActionResult> AddToFavorites([FromBody] AddFavouriteDTO request)
        {
            // Validate the request
            if (request == null || request.UserId == Guid.Empty || request.CarId == Guid.Empty)
            {
                return BadRequest("Invalid request data. UserId and CarId must be provided.");
            }

            try
            {
                // Call the service to add the favorite
                var favoriteResponse = await _favouriteService.AddToFavoritesAsync(request.UserId, request.CarId);
                if (favoriteResponse != null)
                {
                    return Ok(favoriteResponse);
                }

                // If the response is null, the favorite could not be added
                return Conflict("The car is already in your favorites.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Specific error (e.g., car not found)
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET api/favorites/{userId}
        // FavouritesController.cs
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetFavoritesByUserId(Guid userId)
        {
            try
            {
                var favourites = await _favouriteService.GetFavoritesByUserIdAsync(userId);

                if (favourites == null || !favourites.Any())
                {
                    return NotFound(new { message = "No favorites found for the given user." });
                }

                return Ok(favourites);
            }
            catch (Exception ex)
            {
                // Log the exception (for debugging or monitoring purposes)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving favorites.", details = ex.Message });
            }
        }


        [HttpDelete("delete-favorite/{id}")]
        public async Task<IActionResult> DeleteFavourite(Guid id)
        {
            var result = await _favouriteService.DeleteFavouriteAsync(id);

            if (!result)
            {
                return NotFound(new { message = "Favourite not found." });
            }

            return Ok(new { message = "Favourite successfully deleted." });
        }
    }
}
