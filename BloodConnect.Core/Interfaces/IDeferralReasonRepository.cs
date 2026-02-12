using BloodConnect.Core.Entities;

namespace BloodConnect.Core.Interfaces;

public interface IDeferralReasonRepository : IRepository<DeferralReason>
{
    Task<DeferralReason?> GetByCodeAsync(string code);
}

