using BloodConnect.Core.DTOs;
using BloodConnect.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodConnectApi.Controllers;

[ApiController]
[Route("api/deferral-reasons")]
[AllowAnonymous]
public class DeferralReasonsController : ControllerBase
{
    private readonly IDeferralReasonService _deferralReasonService;

    public DeferralReasonsController(IDeferralReasonService deferralReasonService)
    {
        _deferralReasonService = deferralReasonService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeferralReasonResponse>>> GetAllDeferralReasons()
    {
        var reasons = await _deferralReasonService.GetAllDeferralReasonsAsync();
        return Ok(reasons);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DeferralReasonResponse>> GetDeferralReasonById(Guid id)
    {
        try
        {
            var reason = await _deferralReasonService.GetDeferralReasonByIdAsync(id);
            return Ok(reason);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}

