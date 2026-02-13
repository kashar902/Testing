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
        // This method is deprecated - Age-based duplicate checking removed
        // Keeping for backward compatibility but always returns false
        return await Task.FromResult(false);
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

    public async Task<string> GetMaxCouponCodeAsync()
    {
        var allCoupons = await _dbSet
            .Select(d => d.CouponCode)
            .ToListAsync();

        if (!allCoupons.Any())
        {
            return "0000";
        }

        // Filter numeric coupon codes and find the maximum
        var numericCoupons = allCoupons
            .Where(c => int.TryParse(c, out _))
            .Select(c => int.Parse(c))
            .ToList();

        if (!numericCoupons.Any())
        {
            return "0000";
        }

        var maxCode = numericCoupons.Max();
        return maxCode.ToString("D4");
    }
}

