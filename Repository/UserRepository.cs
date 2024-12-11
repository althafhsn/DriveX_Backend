using DriveX_Backend.DB;
using DriveX_Backend.Entities.RentalRequest;
using DriveX_Backend.Entities.Users;
using DriveX_Backend.Entities.Users.UserDTO;
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

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));

            return await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> ResetPasswordChange(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _appDbContext.Entry(user).State = EntityState.Modified;
            await _appDbContext.SaveChangesAsync();
            return user;
        }


        public async Task<User> AuthenticateUserAsync(string username)
        {
            return await _appDbContext.Users
                   .FirstOrDefaultAsync(x => x.NIC == username || x.Licence == username || x.Email == username);
        }

        public async Task<User> ResetPassword(string email)
        {
            var data = await _appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(a => a.Email == email);
            return data;
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

        public async Task<User> GetCustomerByIdAsync(Guid id)
        {
            return await _appDbContext.Users
            .Include(u => u.Addresses)
            .Include(u => u.PhoneNumbers)
            .FirstOrDefaultAsync(u => u.Id == id);
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

        //public async Task UpdateAddressesAsync(Guid userId, List<Address> updatedAddresses)
        //{

        //    var existingAddresses = await _appDbContext.Addresses.Where(p => p.UserId == userId).ToListAsync();

        //    foreach (var address in updatedAddresses)
        //    {
        //        var existingAddress = existingAddresses.FirstOrDefault(p => p.Id ==address.Id);
        //        if (existingAddress != null)
        //        {
        //            existingAddress.HouseNo = existingAddress.HouseNo;
        //            existingAddress.Street1 = existingAddress.Street1;
        //            existingAddress.Street2 = existingAddress.Street2;
        //            existingAddress.City = existingAddress.City;
        //            existingAddress.ZipCode = existingAddress.ZipCode;
        //            existingAddress.Country = existingAddress.Country;

        //        }
        //        else
        //        {
        //            _appDbContext.Addresses.Add(address);

        //        }
        //    }
        //    await _appDbContext.SaveChangesAsync();
        //}

        public async Task UpdateAddressesAsync(Guid userId, List<Address> updatedAddresses)
        {
            // Fetch existing addresses for the user
            var existingAddresses = await _appDbContext.Addresses.Where(p => p.UserId == userId).ToListAsync();

            foreach (var address in updatedAddresses)
            {
                var existingAddress = existingAddresses.FirstOrDefault(p => p.Id == address.Id);
                if (existingAddress != null)
                {
                    // Update existing address with new values
                    existingAddress.HouseNo = address.HouseNo;
                    existingAddress.Street1 = address.Street1;
                    existingAddress.Street2 = address.Street2;
                    existingAddress.City = address.City;
                    existingAddress.ZipCode = address.ZipCode;
                    existingAddress.Country = address.Country;
                }
                else
                {
                    // Add new address if it doesn't exist
                    _appDbContext.Addresses.Add(address);
                }
            }

            // Save changes
            await _appDbContext.SaveChangesAsync();
        }



        public async Task UpdatePhoneNumbersAsync(Guid userId, List<PhoneNumber> phoneNumbers)
        {
            // Fetch the existing phone numbers for the user
            var existingPhoneNumbers = await _appDbContext.PhoneNumbers.Where(p => p.UserId == userId).ToListAsync();

            foreach (var phoneNumber in phoneNumbers)
            {
                var existingPhoneNumber = existingPhoneNumbers.FirstOrDefault(p => p.Id == phoneNumber.Id);
                if (existingPhoneNumber != null)
                {
                    // Update the existing phone number
                    existingPhoneNumber.Mobile1 = phoneNumber.Mobile1;
                }
                else
                {
                    // Add new phone number if it doesn't already exist
                    _appDbContext.PhoneNumbers.Add(phoneNumber);
                }
            }

            // Save changes
            await _appDbContext.SaveChangesAsync();
        }



        public async Task<List<User>> DashboardAllCustomersAsync()
        {
            return await _appDbContext.Set<User>()
       .Include(u => u.Addresses)      // Include related Addresses
       .Include(u => u.PhoneNumbers)  // Include related PhoneNumbers
       .ToListAsync();
        }
        public async Task<User> UpdateCustomerAsync(User customer)
        {
            _appDbContext.Users.Update(customer); // Tracks changes, including related entities
            await _appDbContext.SaveChangesAsync();
            return customer;
        }


        public async Task<User> AddCustomerDashboard(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Customer cannot be null");
            }
            _appDbContext.Users.AddAsync(user);
            await _appDbContext.SaveChangesAsync();
            return user;

        }

        public async Task SaveAsync()
        {
            await _appDbContext.SaveChangesAsync();
        }
        public async Task<bool> DeleteCustomerAsync(Guid id)
        {
            var customer = await _appDbContext.Users.FindAsync(id);
            if (customer == null)
            {
                return false; // Customer not found
            }

            _appDbContext.Users.Remove(customer);
            await _appDbContext.SaveChangesAsync();
            return true; // Customer deleted successfully
        }
        public async Task<IEnumerable<RentalRequest>> GetRentalRequestsByCustomerIdAsync(Guid customerId)
        {
            return await _appDbContext.RentalRequests
                .Include(r => r.Car)
                .Where(r => r.UserId == customerId)
                .ToListAsync();
        }

        public async Task<List<User>> GetAllManagersAsync()
        {
            return await _appDbContext.Set<User>()
                .Include(u => u.Addresses)      // Include related Addresses
                .Include(u => u.PhoneNumbers)  // Include related PhoneNumbers
                .Where(u => u.Role == Role.Manager) // Filter by Manager role
                .ToListAsync();
        }
        public async Task<User> UpdateManagerAsync(User manager)
        {
            _appDbContext.Users.Update(manager); // Tracks changes
            await _appDbContext.SaveChangesAsync();
            return manager;
        }


        public async Task<User> GetManagerByIdAsync(Guid id)
        {
            return await _appDbContext.Users
                .Include(m => m.Addresses)
                .Include(m => m.PhoneNumbers)
                .FirstOrDefaultAsync(m => m.Id == id && m.Role == Role.Manager);
        }

        public async Task<User> AddManagerDashboard(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Manager cannot be null");
            }
            await _appDbContext.Users.AddAsync(user);
            await _appDbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> GetUserByNICAndRoleAsync(string nic, Role role)
        {
            return await _appDbContext.Users
                .FirstOrDefaultAsync(u => u.NIC == nic && u.Role == role);
        }
        public async Task UpdateUserAsync(User user)
        {
            _appDbContext.Users.Update(user);
            await _appDbContext.SaveChangesAsync();
        }
        public async Task<List<Address>> GetAddressesByCustomerIdAsync(Guid customerId)
        {
            return await _appDbContext.Addresses
                .Where(a => a.UserId == customerId)
                .ToListAsync();
        }

        // Add a new address
        public async Task<Address> AddAddressAsync(Address address)
        {
            _appDbContext.Addresses.Add(address);
            await _appDbContext.SaveChangesAsync();
            return address;
        }
        /*  public async Task UpdateAddressAsync(Address address)
          {
              _appDbContext.Addresses.Update(address);
              await _appDbContext.SaveChangesAsync();
          }*/
        public async Task<Address> GetAddressByIdAsync(Guid Id)
        {
            return await _appDbContext.Addresses.FirstOrDefaultAsync(a => a.Id == Id);
        }

        public async Task UpdateAddressAsync(Address address)
        {
            _appDbContext.Set<Address>().Update(address);
            await _appDbContext.SaveChangesAsync();
        }
        /*
                public async Task<PhoneNumber>Addphonenumber(PhoneNumber phoneNumber)
                {
                    _appDbContext.PhoneNumbers.Add(phoneNumber);
                    await _appDbContext.SaveChangesAsync();
                    return phoneNumber;

                }*/
        public async Task<bool> DeleteAddressAsync(Address address)
        {
            if (address == null)
            {
                return false;
            }

            _appDbContext.Set<Address>().Remove(address);
            var result = await _appDbContext.SaveChangesAsync();
            return result > 0;
        }
        public async Task<PhoneNumber> AddPhoneNumberAsync(PhoneNumber phoneNumber)
        {
            await _appDbContext.PhoneNumbers.AddAsync(phoneNumber);
            await _appDbContext.SaveChangesAsync();
            return phoneNumber;
        }

        // Update Phone Number
        public async Task<PhoneNumber> UpdatePhoneNumberAsync(PhoneNumber phoneNumber)
        {
            _appDbContext.PhoneNumbers.Update(phoneNumber);
            await _appDbContext.SaveChangesAsync();
            return phoneNumber;
        }

        // Delete Phone Number
        public async Task<bool> DeletePhoneNumberAsync(Guid phoneNumberId)
        {
            var phoneNumber = await _appDbContext.PhoneNumbers.FindAsync(phoneNumberId);
            if (phoneNumber == null) return false;

            _appDbContext.PhoneNumbers.Remove(phoneNumber);
            await _appDbContext.SaveChangesAsync();
            return true;
        }

        // Get Phone Number by ID
        public async Task<PhoneNumber?> GetPhoneNumberByIdAsync(Guid phoneNumberId)
        {
            return await _appDbContext.PhoneNumbers.FirstOrDefaultAsync(p => p.Id == phoneNumberId);
        }
        public async Task<List<PhoneNumber>> GetPhoneNumbersByCustomerIdAsync(Guid customerId)
        {
            return await _appDbContext.PhoneNumbers
                                 .Where(p => p.UserId == customerId)
                                 .ToListAsync();
        }

    }


}

