using BloodConnect.Core.DTOs;
using BloodConnect.Core.Entities;
using BloodConnect.Core.Interfaces;

namespace BloodConnect.Services.Services;

public class BranchService : IBranchService
{
    private readonly IUnitOfWork _unitOfWork;

    public BranchService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<BranchResponse>> GetActiveBranchesAsync()
    {
        var branches = await _unitOfWork.Branches.GetActiveBranchesAsync();
        return branches.Select(MapToResponse);
    }

    public async Task<BranchResponse> GetBranchByIdAsync(Guid id)
    {
        var branch = await _unitOfWork.Branches.GetByIdAsync(id);
        if (branch == null)
        {
            throw new KeyNotFoundException($"Branch with ID {id} not found");
        }

        return MapToResponse(branch);
    }

    public async Task<BranchResponse> CreateBranchAsync(CreateBranchRequest request)
    {
        var branch = new Branch
        {
            BranchId = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Address = request.Address.Trim(),
            IsActive = request.IsActive
        };

        await _unitOfWork.Branches.AddAsync(branch);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(branch);
    }

    public async Task<BranchResponse> UpdateBranchAsync(Guid id, UpdateBranchRequest request)
    {
        var branch = await _unitOfWork.Branches.GetByIdAsync(id);
        if (branch == null)
        {
            throw new KeyNotFoundException($"Branch with ID {id} not found");
        }

        branch.Name = request.Name.Trim();
        branch.Address = request.Address.Trim();
        branch.IsActive = request.IsActive;

        await _unitOfWork.Branches.UpdateAsync(branch);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(branch);
    }

    private BranchResponse MapToResponse(Branch branch)
    {
        return new BranchResponse
        {
            BranchId = branch.BranchId,
            Name = branch.Name,
            Address = branch.Address,
            IsActive = branch.IsActive
        };
    }
}

