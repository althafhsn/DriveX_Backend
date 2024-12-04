namespace DriveX_Backend.Entities.RentalRequest.Models
{
    public class getCustomerRentalDTO
    {
        public Guid Id { get; set; }
        public Guid CarId { get; set; }
        public Guid UserId { get; set; }
        public string RegNo { get; set; }
        public string BrandName { get; set; }
        public string ModelName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Action { get; set; }
        public string Status { get; set; }
    }
}
