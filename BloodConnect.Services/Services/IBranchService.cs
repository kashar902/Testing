using BloodConnect.Core.DTOs;

namespace BloodConnect.Services.Services;

public interface IBranchService
{
    Task<IEnumerable<BranchResponse>> GetActiveBranchesAsync();
    Task<BranchResponse> GetBranchByIdAsync(Guid id);
    Task<BranchResponse> CreateBranchAsync(CreateBranchRequest request);
    Task<BranchResponse> UpdateBranchAsync(Guid id, UpdateBranchRequest request);
}

