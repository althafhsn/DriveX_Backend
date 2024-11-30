
namespace DriveX_Backend.Entities.RentalRequest.Models
{
    public class AddRentalRequestDTO
    {
        public Guid CarId { get; set; }
        public Guid UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Action { get; set; }
        public string Status { get; set; }
    }

    public class AddRentalResponseDTO
    {
        public Guid Id { get; set; }
        public Guid CarId { get; set; }
        public Guid UserId { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Today;
        public int Duration { get; set; }
        public decimal TotalPrice { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Action { get; set; }
        public string Status { get; set; }


    }
}
