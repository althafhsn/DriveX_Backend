﻿using DriveX_Backend.DB;
using DriveX_Backend.Entities.RentalRequest;
using DriveX_Backend.IRepository;
using Microsoft.EntityFrameworkCore;

namespace DriveX_Backend.Repository
{
    public class RentalRequestRepository:IRentalRequestRepository
    {
        private readonly AppDbContext _context;
        public RentalRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddRentalRequestAsync(RentalRequest rentalRequest)
        {
            _context.RentalRequests.Add(rentalRequest);
            await _context.SaveChangesAsync();
        }

        public async Task<RentalRequest> GetByIdAsync(Guid id)
        {
            return await _context.Set<RentalRequest>().FindAsync(id);
        }

        public async Task UpdateAsync(RentalRequest rentalRequest)
        {
            _context.Set<RentalRequest>().Update(rentalRequest);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRentalRequestAsync(RentalRequest rentalRequest)
        {
            _context.RentalRequests.Update(rentalRequest);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<RentalRequest>> GetAllRentalRequestsAsync()
        {
            return await _context.RentalRequests.ToListAsync();
        }
    }
}