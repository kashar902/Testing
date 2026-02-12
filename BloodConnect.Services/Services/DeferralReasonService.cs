using BloodConnect.Core.DTOs;
using BloodConnect.Core.Entities;
using BloodConnect.Core.Interfaces;

namespace BloodConnect.Services.Services;

public class DeferralReasonService : IDeferralReasonService
{
    private readonly IUnitOfWork _unitOfWork;

    public DeferralReasonService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<DeferralReasonResponse>> GetAllDeferralReasonsAsync()
    {
        var reasons = await _unitOfWork.DeferralReasons.GetAllAsync();
        return reasons.Select(MapToResponse);
    }

    public async Task<DeferralReasonResponse> GetDeferralReasonByIdAsync(Guid id)
    {
        var reason = await _unitOfWork.DeferralReasons.GetByIdAsync(id);
        if (reason == null)
        {
            throw new KeyNotFoundException($"Deferral reason with ID {id} not found");
        }

        return MapToResponse(reason);
    }

    private DeferralReasonResponse MapToResponse(DeferralReason reason)
    {
        return new DeferralReasonResponse
        {
            DeferralReasonId = reason.DeferralReasonId,
            Code = reason.Code,
            Label = reason.Label,
            Category = reason.Category,
            DefaultDurationDays = reason.DefaultDurationDays
        };
    }
}

