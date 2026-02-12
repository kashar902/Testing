using BloodConnect.Core.DTOs;
using BloodConnect.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodConnectApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DonorsController : ControllerBase
{
    private readonly IDonorService _donorService;

    public DonorsController(IDonorService donorService)
    {
        _donorService = donorService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<DonorResponse>> CreateDonor([FromBody] CreateDonorRequest request)
    {
        try
        {
            var donor = await _donorService.CreateDonorAsync(request);
            return CreatedAtAction(nameof(GetDonorById), new { id = donor.DonorId }, donor);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<DonorResponse>>> GetDonors([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var donors = await _donorService.GetDonorsAsync(page, pageSize);
        return Ok(donors);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DonorResponse>> GetDonorById(Guid id)
    {
        try
        {
            var donor = await _donorService.GetDonorByIdAsync(id);
            return Ok(donor);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet("coupon/{couponCode}")]
    [AllowAnonymous]
    public async Task<ActionResult<DonorResponse>> GetDonorByCoupon(string couponCode)
    {
        var donor = await _donorService.GetDonorByCouponCodeAsync(couponCode);
        if (donor == null)
        {
            return NotFound(new { error = "No donor found with this coupon code" });
        }
        return Ok(donor);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DonorResponse>> UpdateDonor(Guid id, [FromBody] UpdateDonorRequest request)
    {
        try
        {
            var donor = await _donorService.UpdateDonorAsync(id, request);
            return Ok(donor);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet("{id}/screenings")]
    public async Task<ActionResult<IEnumerable<ScreeningResponse>>> GetDonorScreenings(Guid id)
    {
        try
        {
            var screenings = await _donorService.GetDonorScreeningsAsync(id);
            return Ok(screenings);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet("next-coupon")]
    [AllowAnonymous]
    public async Task<ActionResult<NextCouponCodeResponse>> GetNextCouponCode()
    {
        var couponCode = await _donorService.GetNextCouponCodeAsync();
        return Ok(new NextCouponCodeResponse { CouponCode = couponCode });
    }
}

