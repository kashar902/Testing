using BloodConnect.Core.Entities;
using BloodConnect.Core.Interfaces;
using BloodConnect.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodConnect.Infrastructure.Repositories;

public class ScreeningRepository : Repository<DonationScreening>, IScreeningRepository
{
    public ScreeningRepository(BloodConnectDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<DonationScreening>> GetByDonorIdAsync(Guid donorId)
    {
        return await _dbSet
            .Where(s => s.DonorId == donorId)
            .Include(s => s.Donor)
            .Include(s => s.Branch)
            .Include(s => s.DeferralReason)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<DonationScreening>> GetByBranchIdAsync(Guid branchId)
    {
        return await _dbSet
            .Where(s => s.BranchId == branchId)
            .Include(s => s.Donor)
            .Include(s => s.DeferralReason)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }
}

