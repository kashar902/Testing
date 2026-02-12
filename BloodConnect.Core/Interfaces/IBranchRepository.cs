using BloodConnect.Core.Entities;

namespace BloodConnect.Core.Interfaces;

public interface IBranchRepository : IRepository<Branch>
{
    Task<IEnumerable<Branch>> GetActiveBranchesAsync();
}

