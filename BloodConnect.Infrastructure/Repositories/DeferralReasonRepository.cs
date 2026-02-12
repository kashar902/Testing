using BloodConnect.Core.Entities;
using BloodConnect.Core.Interfaces;
using BloodConnect.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodConnect.Infrastructure.Repositories;

public class DeferralReasonRepository : Repository<DeferralReason>, IDeferralReasonRepository
{
    public DeferralReasonRepository(BloodConnectDbContext context) : base(context)
    {
    }

    public async Task<DeferralReason?> GetByCodeAsync(string code)
    {
        return await _dbSet
            .FirstOrDefaultAsync(dr => dr.Code.ToUpper() == code.ToUpper());
    }
}

