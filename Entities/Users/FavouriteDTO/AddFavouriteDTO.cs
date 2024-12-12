namespace DriveX_Backend.Entities.Users.FavouriteDTO
{
    public class AddFavouriteDTO
    {
        public Guid UserId { get; set; }
        public Guid CarId { get; set; }
    }

    public class AddFavouriteResponseDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CarId { get; set; }
    }
}
