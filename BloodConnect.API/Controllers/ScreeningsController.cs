using BloodConnect.Core.DTOs;
using BloodConnect.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodConnectApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ScreeningsController : ControllerBase
{
    private readonly IScreeningService _screeningService;

    public ScreeningsController(IScreeningService screeningService)
    {
        _screeningService = screeningService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<ScreeningResponse>> CreateScreening([FromBody] CreateScreeningRequest request)
    {
        try
        {
            var screening = await _screeningService.CreateScreeningAsync(request);
            return CreatedAtAction(nameof(GetScreeningById), new { id = screening.ScreeningId }, screening);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<ScreeningResponse>>> GetScreenings([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var screenings = await _screeningService.GetScreeningsAsync(page, pageSize);
        return Ok(screenings);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ScreeningResponse>> GetScreeningById(Guid id)
    {
        try
        {
            var screening = await _screeningService.GetScreeningByIdAsync(id);
            return Ok(screening);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet("donor/{donorId}")]
    public async Task<ActionResult<IEnumerable<ScreeningResponse>>> GetScreeningsByDonor(Guid donorId)
    {
        try
        {
            var screenings = await _screeningService.GetScreeningsByDonorIdAsync(donorId);
            return Ok(screenings);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}

