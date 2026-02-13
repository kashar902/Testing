using BloodConnect.Core.DTOs;

namespace BloodConnect.Services.Services;

public interface IPrinterService
{
    Task<PrintResponse> PrintDonorSlipsAsync(PrintDonorSlipsRequest request);
    Task<PrintResponse> PrintTestPageAsync();
    Task<bool> CheckPrinterHealthAsync();
    Task<List<PrinterInfo>> GetAvailablePrintersAsync();
}
