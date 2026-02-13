using BloodConnect.Core.DTOs;
using BloodConnect.Core.Entities;
using BloodConnect.Core.Interfaces;

namespace BloodConnect.Services.Services;

public class DonorService : IDonorService
{
    private readonly IUnitOfWork _unitOfWork;

    public DonorService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DonorResponse> CreateDonorAsync(CreateDonorRequest request)
    {
        // Validate national ID uniqueness
        if (await _unitOfWork.Donors.ExistsByNationalIdAsync(request.NationalId))
        {
            throw new InvalidOperationException("A donor with this National ID already exists.");
        }

        // Validate age
        if (request.Age < 18 || request.Age > 100)
        {
            throw new ArgumentException("Age must be between 18 and 100 years");
        }

        // Auto-generate coupon code
        var couponCode = await GetNextCouponCodeAsync();

        var now = DateTime.UtcNow;
        var donor = new Donor
        {
            DonorId = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            FatherHusbandName = request.FatherHusbandName?.Trim(),
            Age = request.Age,
            Gender = request.Gender.ToLower(),
            BloodGroup = request.BloodGroup?.Trim(),
            Phone = request.Phone.Trim(),
            Email = request.Email.Trim(),
            NationalId = request.NationalId.Trim(),
            District = request.District?.Trim(),
            Address = new Address
            {
                Line1 = request.Address.Line1,
                City = request.Address.City,
                Province = request.Address.Province,
                Country = request.Address.Country,
                PostalCode = request.Address.PostalCode
            },
            TimesDonatedBefore = request.TimesDonatedBefore,
            SourceOfInfo = request.SourceOfInfo?.Trim(),
            CouponCode = couponCode,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _unitOfWork.Donors.AddAsync(donor);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(donor);
    }

    public async Task<DonorResponse> GetDonorByIdAsync(Guid id)
    {
        var donor = await _unitOfWork.Donors.GetByIdAsync(id);
        if (donor == null)
        {
            throw new KeyNotFoundException($"Donor with ID {id} not found");
        }

        return MapToResponse(donor);
    }

    public async Task<DonorResponse?> GetDonorByCouponCodeAsync(string couponCode)
    {
        var donor = await _unitOfWork.Donors.GetByCouponCodeAsync(couponCode);
        return donor == null ? null : MapToResponse(donor);
    }

    public async Task<PaginatedResponse<DonorResponse>> GetDonorsAsync(int page, int pageSize)
    {
        var donors = await _unitOfWork.Donors.GetPagedAsync(page, pageSize);
        var totalCount = await _unitOfWork.Donors.CountAsync();

        return new PaginatedResponse<DonorResponse>
        {
            Data = donors.Select(MapToResponse).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<DonorResponse> UpdateDonorAsync(Guid id, UpdateDonorRequest request)
    {
        var donor = await _unitOfWork.Donors.GetByIdAsync(id);
        if (donor == null)
        {
            throw new KeyNotFoundException($"Donor with ID {id} not found");
        }

        donor.FullName = request.FullName.Trim();
        donor.Phone = request.Phone.Trim();
        donor.Email = request.Email.Trim();
        donor.Address = new Address
        {
            Line1 = request.Address.Line1,
            City = request.Address.City,
            Province = request.Address.Province,
            Country = request.Address.Country,
            PostalCode = request.Address.PostalCode
        };
        donor.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Donors.UpdateAsync(donor);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(donor);
    }

    public async Task<IEnumerable<ScreeningResponse>> GetDonorScreeningsAsync(Guid id)
    {
        var donor = await _unitOfWork.Donors.GetByIdAsync(id);
        if (donor == null)
        {
            throw new KeyNotFoundException($"Donor with ID {id} not found");
        }

        var screenings = await _unitOfWork.Donors.GetDonorScreeningsAsync(id);
        return screenings.Select(MapScreeningToResponse);
    }

    public async Task<string> GetNextCouponCodeAsync()
    {
        var maxCouponCode = await _unitOfWork.Donors.GetMaxCouponCodeAsync();
        var maxNumber = int.Parse(maxCouponCode);
        var nextNumber = maxNumber + 1;
        return nextNumber.ToString("D4");
    }

    private DonorResponse MapToResponse(Donor donor)
    {
        return new DonorResponse
        {
            DonorId = donor.DonorId,
            FullName = donor.FullName,
            FatherHusbandName = donor.FatherHusbandName,
            Age = donor.Age,
            Gender = donor.Gender,
            BloodGroup = donor.BloodGroup,
            Phone = donor.Phone,
            Email = donor.Email,
            NationalId = donor.NationalId,
            District = donor.District,
            Address = new AddressDto
            {
                Line1 = donor.Address.Line1,
                City = donor.Address.City,
                Province = donor.Address.Province,
                Country = donor.Address.Country,
                PostalCode = donor.Address.PostalCode
            },
            TimesDonatedBefore = donor.TimesDonatedBefore,
            SourceOfInfo = donor.SourceOfInfo,
            CouponCode = donor.CouponCode,
            CreatedAt = donor.CreatedAt,
            UpdatedAt = donor.UpdatedAt,
            LastDonationDate = donor.LastDonationDate?.ToString("yyyy-MM-dd")
        };
    }

    private ScreeningResponse MapScreeningToResponse(DonationScreening screening)
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

