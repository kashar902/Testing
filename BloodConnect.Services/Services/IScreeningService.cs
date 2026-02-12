using BloodConnect.Core.DTOs;

namespace BloodConnect.Services.Services;

public interface IScreeningService
{
    Task<ScreeningResponse> CreateScreeningAsync(CreateScreeningRequest request);
    Task<ScreeningResponse> GetScreeningByIdAsync(Guid id);
    Task<PaginatedResponse<ScreeningResponse>> GetScreeningsAsync(int page, int pageSize);
    Task<IEnumerable<ScreeningResponse>> GetScreeningsByDonorIdAsync(Guid donorId);
}

