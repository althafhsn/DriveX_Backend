namespace DriveX_Backend.Entities.RentalRequest.Models
{
    public class recentRentalRequestDTO
    {
        public Guid Id { get; set; }
        public Guid CarId {get; set;}
        public Guid UserId { get; set;}
        public string RegNo { get; set;}
        public string FirstName { get; set;}
        public string LastName { get; set;}
        public string Status { get; set;}
        public decimal TotalPrice { get; set;}
    }
}
