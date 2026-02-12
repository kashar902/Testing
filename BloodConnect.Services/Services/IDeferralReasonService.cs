using BloodConnect.Core.DTOs;

namespace BloodConnect.Services.Services;

public interface IDeferralReasonService
{
    Task<IEnumerable<DeferralReasonResponse>> GetAllDeferralReasonsAsync();
    Task<DeferralReasonResponse> GetDeferralReasonByIdAsync(Guid id);
}

