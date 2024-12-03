namespace DriveX_Backend.Entities.RentalRequest.Models
{
    public class OngoingRentalsDTO
    {
        public Guid Id { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Today;
        public Guid CarId { get; set; }
        public Guid UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public string RegNo { get; set; }
        public string NIC { get; set; }
    }
}
