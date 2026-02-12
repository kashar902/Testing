using BloodConnect.Core.Entities;
using BloodConnect.Core.Interfaces;
using BloodConnect.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodConnect.Infrastructure.Repositories;

public class BranchRepository : Repository<Branch>, IBranchRepository
{
    public BranchRepository(BloodConnectDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Branch>> GetActiveBranchesAsync()
    {
        return await _dbSet
            .Where(b => b.IsActive)
            .OrderBy(b => b.Name)
            .ToListAsync();
    }
}

