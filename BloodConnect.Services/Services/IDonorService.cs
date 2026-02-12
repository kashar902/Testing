using BloodConnect.Core.DTOs;

namespace BloodConnect.Services.Services;

public interface IDonorService
{
    Task<DonorResponse> CreateDonorAsync(CreateDonorRequest request);
    Task<DonorResponse> GetDonorByIdAsync(Guid id);
    Task<DonorResponse?> GetDonorByCouponCodeAsync(string couponCode);
    Task<PaginatedResponse<DonorResponse>> GetDonorsAsync(int page, int pageSize);
    Task<DonorResponse> UpdateDonorAsync(Guid id, UpdateDonorRequest request);
    Task<IEnumerable<ScreeningResponse>> GetDonorScreeningsAsync(Guid id);
    Task<string> GetNextCouponCodeAsync();
}

