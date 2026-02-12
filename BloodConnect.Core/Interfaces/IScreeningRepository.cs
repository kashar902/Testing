using BloodConnect.Core.Entities;

namespace BloodConnect.Core.Interfaces;

public interface IScreeningRepository : IRepository<DonationScreening>
{
    Task<IEnumerable<DonationScreening>> GetByDonorIdAsync(Guid donorId);
    Task<IEnumerable<DonationScreening>> GetByBranchIdAsync(Guid branchId);
}

