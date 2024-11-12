using DriveX_Backend.DB;
using DriveX_Backend.Entities.Users;
using DriveX_Backend.Helpers;
using DriveX_Backend.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

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
                   .FirstOrDefaultAsync(x => x.NIC == username || x.Licence == username);
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
    }
}
