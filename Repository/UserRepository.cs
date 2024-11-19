using DriveX_Backend.DB;
using DriveX_Backend.Entities.Users;

using DriveX_Backend.IRepository;

using Microsoft.EntityFrameworkCore;


namespace DriveX_Backend.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _appDbContext;

        public UserRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<User> AddUserAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null");
            }

            await _appDbContext.Users.AddAsync(user);
            await _appDbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> GetUserByNICAsync(string nic)
        {
            return await _appDbContext.Users.FirstOrDefaultAsync(u => u.NIC == nic);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> AuthenticateUserAsync(string username)
        {
            return await _appDbContext.Users
                   .FirstOrDefaultAsync(x => x.NIC == username || x.Licence == username || x.Email == username);
        }

        public async Task<bool> RefreshTokenExistsAsync(string refreshToken)
        {
            return await _appDbContext.Users
                .AnyAsync(a => a.RefreshToken == refreshToken);
        }

        public async Task<IEnumerable<User>> AddUsersAsync(IEnumerable<User> users)
        {
            if (users == null || !users.Any())
            {
                throw new ArgumentException("User list cannot be null or empty.");
            }

            await _appDbContext.Users.AddRangeAsync(users);
            await _appDbContext.SaveChangesAsync();

            return users;
        }

        public async Task<User> GetCustomerByIdAsync (Guid id)
        {
            return await _appDbContext.Users.FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _appDbContext.Users.ToListAsync();
        }
        public async Task<bool> UpdateUserRefreshTokenAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User object cannot be null");
            }

            var existingUser = await _appDbContext.Users.FindAsync(user.Id);
            if (existingUser == null)
            {
                return false; // User not found
            }

            existingUser.RefreshToken = user.RefreshToken;
            existingUser.Token = user.Token; // If needed to persist access tokens

            _appDbContext.Users.Update(existingUser);
            await _appDbContext.SaveChangesAsync();

            return true; // Indicates success
        }

    }
}
