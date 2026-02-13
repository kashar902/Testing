using BloodConnect.Core.DTOs;
using BloodConnect.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodConnectApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class PrinterController : ControllerBase
{
    private readonly IPrinterService _printerService;
    private readonly ILogger<PrinterController> _logger;

    public PrinterController(IPrinterService printerService, ILogger<PrinterController> logger)
    {
        _printerService = printerService;
        _logger = logger;
    }

    /// <summary>
    /// Health check endpoint for printer service
    /// </summary>
    [HttpGet("health")]
    [AllowAnonymous]
    public async Task<ActionResult> GetHealth()
    {
        var isHealthy = await _printerService.CheckPrinterHealthAsync();
        
        return Ok(new
        {
            status = isHealthy ? "ok" : "unavailable",
            service = "Blood Connect Printer Service",
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Get list of available printers
    /// </summary>
    [HttpGet("list")]
    public async Task<ActionResult<List<PrinterInfo>>> GetPrinters()
    {
        try
        {
            var printers = await _printerService.GetAvailablePrintersAsync();
            return Ok(new
            {
                success = true,
                printers
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list printers");
            return StatusCode(500, new
            {
                success = false,
                error = "Failed to list printers",
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Print donor slips (Donor Copy, Lucky Draw, Office Copy) with automatic paper cuts
    /// </summary>
    [HttpPost("print-donor-slips")]
    public async Task<ActionResult<PrintResponse>> PrintDonorSlips([FromBody] PrintDonorSlipsRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.FullName) ||
                string.IsNullOrWhiteSpace(request.NationalId) ||
                string.IsNullOrWhiteSpace(request.CouponCode))
            {
                return BadRequest(new PrintResponse
                {
                    Success = false,
                    Error = "Missing required fields: FullName, NationalId, CouponCode"
                });
            }

            var result = await _printerService.PrintDonorSlipsAsync(request);

            if (!result.Success)
            {
                return StatusCode(500, result);
            }

            _logger.LogInformation("Printed slips for donor {FullName} with coupon {CouponCode}",
                request.FullName, request.CouponCode);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error printing donor slips");
            return StatusCode(500, new PrintResponse
            {
                Success = false,
                Error = "Server error",
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Print a test page to verify printer is working
    /// </summary>
    [HttpPost("test")]
    public async Task<ActionResult<PrintResponse>> PrintTest()
    {
        try
        {
            var result = await _printerService.PrintTestPageAsync();

            if (!result.Success)
            {
                return StatusCode(500, result);
            }

            _logger.LogInformation("Test print completed successfully");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Test print failed");
            return StatusCode(500, new PrintResponse
            {
                Success = false,
                Error = "Test print failed",
                Message = ex.Message
            });
        }
    }
}
