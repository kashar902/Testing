using BloodConnect.Core.DTOs;
using BloodConnect.Core.Entities;
using BloodConnect.Core.Interfaces;

namespace BloodConnect.Services.Services;

public class ScreeningService : IScreeningService
{
    private readonly IUnitOfWork _unitOfWork;

    public ScreeningService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ScreeningResponse> CreateScreeningAsync(CreateScreeningRequest request)
    {
        // Verify donor exists
        var donor = await _unitOfWork.Donors.GetByIdAsync(request.DonorId);
        if (donor == null)
        {
            throw new KeyNotFoundException($"Donor with ID {request.DonorId} not found");
        }

        // Verify branch exists
        var branch = await _unitOfWork.Branches.GetByIdAsync(request.BranchId);
        if (branch == null)
        {
            throw new KeyNotFoundException($"Branch with ID {request.BranchId} not found");
        }

        // Verify deferral reason if deferred
        if (request.EligibilityStatus == "deferred" && request.DeferralReasonId.HasValue)
        {
            var deferralReason = await _unitOfWork.DeferralReasons.GetByIdAsync(request.DeferralReasonId.Value);
            if (deferralReason == null)
            {
                throw new KeyNotFoundException($"Deferral reason with ID {request.DeferralReasonId} not found");
            }
        }

        DateTime? deferralUntil = null;
        if (!string.IsNullOrEmpty(request.DeferralUntil) && DateTime.TryParse(request.DeferralUntil, out var parsedDate))
        {
            deferralUntil = parsedDate;
        }

        var screening = new DonationScreening
        {
            ScreeningId = Guid.NewGuid(),
            DonorId = request.DonorId,
            BranchId = request.BranchId,
            StaffId = request.StaffId,
            Vitals = new Vitals
            {
                BpSystolic = request.Vitals.BpSystolic,
                BpDiastolic = request.Vitals.BpDiastolic,
                Pulse = request.Vitals.Pulse,
                TempC = request.Vitals.TempC,
                WeightKg = request.Vitals.WeightKg,
                HbGdl = request.Vitals.HbGdl
            },
            Notes = request.Notes.Trim(),
            EligibilityStatus = request.EligibilityStatus.ToLower(),
            DeferralReasonId = request.DeferralReasonId,
            DeferralUntil = deferralUntil,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Screenings.AddAsync(screening);

        // Update donor's last donation date if eligible
        if (screening.EligibilityStatus == "eligible")
        {
            donor.LastDonationDate = screening.CreatedAt;
            donor.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Donors.UpdateAsync(donor);
        }

        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(screening);
    }

    public async Task<ScreeningResponse> GetScreeningByIdAsync(Guid id)
    {
        var screening = await _unitOfWork.Screenings.GetByIdAsync(id);
        if (screening == null)
        {
            throw new KeyNotFoundException($"Screening with ID {id} not found");
        }

        return MapToResponse(screening);
    }

    public async Task<PaginatedResponse<ScreeningResponse>> GetScreeningsAsync(int page, int pageSize)
    {
        var screenings = await _unitOfWork.Screenings.GetPagedAsync(page, pageSize);
        var totalCount = await _unitOfWork.Screenings.CountAsync();

        return new PaginatedResponse<ScreeningResponse>
        {
            Data = screenings.Select(MapToResponse).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<IEnumerable<ScreeningResponse>> GetScreeningsByDonorIdAsync(Guid donorId)
    {
        var donor = await _unitOfWork.Donors.GetByIdAsync(donorId);
        if (donor == null)
        {
            throw new KeyNotFoundException($"Donor with ID {donorId} not found");
        }

        var screenings = await _unitOfWork.Screenings.GetByDonorIdAsync(donorId);
        return screenings.Select(MapToResponse);
    }

    private ScreeningResponse MapToResponse(DonationScreening screening)
    {
        return new ScreeningResponse
        {
            ScreeningId = screening.ScreeningId,
            DonorId = screening.DonorId,
            BranchId = screening.BranchId,
            StaffId = screening.StaffId,
            Vitals = new VitalsDto
            {
                BpSystolic = screening.Vitals.BpSystolic,
                BpDiastolic = screening.Vitals.BpDiastolic,
                Pulse = screening.Vitals.Pulse,
                TempC = screening.Vitals.TempC,
                WeightKg = screening.Vitals.WeightKg,
                HbGdl = screening.Vitals.HbGdl
            },
            Notes = screening.Notes,
            EligibilityStatus = screening.EligibilityStatus,
            DeferralReasonId = screening.DeferralReasonId,
            DeferralUntil = screening.DeferralUntil?.ToString("yyyy-MM-dd"),
            CreatedAt = screening.CreatedAt
        };
    }
}

