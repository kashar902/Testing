using BloodConnect.Core.DTOs;
using BloodConnect.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodConnectApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BranchesController : ControllerBase
{
    private readonly IBranchService _branchService;

    public BranchesController(IBranchService branchService)
    {
        _branchService = branchService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<BranchResponse>>> GetActiveBranches()
    {
        var branches = await _branchService.GetActiveBranchesAsync();
        return Ok(branches);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BranchResponse>> GetBranchById(Guid id)
    {
        try
        {
            var branch = await _branchService.GetBranchByIdAsync(id);
            return Ok(branch);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BranchResponse>> CreateBranch([FromBody] CreateBranchRequest request)
    {
        var branch = await _branchService.CreateBranchAsync(request);
        return CreatedAtAction(nameof(GetBranchById), new { id = branch.BranchId }, branch);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BranchResponse>> UpdateBranch(Guid id, [FromBody] UpdateBranchRequest request)
    {
        try
        {
            var branch = await _branchService.UpdateBranchAsync(id, request);
            return Ok(branch);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}

