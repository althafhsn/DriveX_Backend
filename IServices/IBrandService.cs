namespace DriveX_Backend.IServices
{
    public interface IBrandService
    {
        Task<List<string>> GetOrAddModelsByBrandNameAsync(string brandName, List<string> modelNames);
    }
}
