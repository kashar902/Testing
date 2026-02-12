using BloodConnect.Core.Entities;
using BloodConnect.Core.Interfaces;
using BloodConnect.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodConnect.Infrastructure.Repositories;

public class DonorRepository : Repository<Donor>, IDonorRepository
{
    public DonorRepository(BloodConnectDbContext context) : base(context)
    {
    }

    public async Task<Donor?> GetByCouponCodeAsync(string couponCode)
    {
        return await _dbSet
            .FirstOrDefaultAsync(d => d.CouponCode.ToUpper() == couponCode.ToUpper());
    }

    public async Task<bool> ExistsByCouponCodeAsync(string couponCode)
    {
        return await _dbSet
            .AnyAsync(d => d.CouponCode.ToUpper() == couponCode.ToUpper());
    }

    public async Task<bool> ExistsByNationalIdAsync(string nationalId)
    {
        return await _dbSet
            .AnyAsync(d => d.NationalId == nationalId);
    }

    public async Task<bool> ExistsByPhoneAndDobAsync(string phone, DateTime dob)
    {
        return await _dbSet
            .AnyAsync(d => d.Phone == phone && d.DateOfBirth.Date == dob.Date);
    }

    public async Task<IEnumerable<DonationScreening>> GetDonorScreeningsAsync(Guid donorId)
    {
        return await _context.DonationScreenings
            .Where(s => s.DonorId == donorId)
            .Include(s => s.Branch)
            .Include(s => s.DeferralReason)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }
}

