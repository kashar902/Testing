using System.Drawing.Printing;
using BloodConnect.Core.DTOs;
using BloodConnect.Services.Helpers;
using ESCPOS_NET.Emitters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BloodConnect.Services.Services;

public class PrinterService : IPrinterService
{
    private readonly ILogger<PrinterService> _logger;
    private readonly string _printerName;
    private static readonly TimeZoneInfo PakistanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");


    public PrinterService(ILogger<PrinterService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _printerName = configuration["Printer:Name"] ?? "EPSON TM-T88V Receipt";
    }

    public async Task<bool> CheckPrinterHealthAsync()
    {
        try
        {
            var printers = await GetAvailablePrintersAsync();
            return printers.Any(p => p.IsConnected);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check printer health");
            return false;
        }
    }

    public async Task<List<PrinterInfo>> GetAvailablePrintersAsync()
    {
        return await Task.Run(() =>
        {
            var printers = new List<PrinterInfo>();
            
            try
            {
                // Get all local printers (Windows only)
                if (OperatingSystem.IsWindows())
                {
                    var localPrinters = PrinterSettings.InstalledPrinters;
                    
                    foreach (string printerName in localPrinters)
                    {
                        printers.Add(new PrinterInfo
                        {
                            Name = printerName,
                            IsConnected = true,
                            VendorId = "N/A",
                            ProductId = "N/A"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list printers");
            }

            return printers;
        });
    }
    
    private DateTime ConvertToPakistanTime(DateTime utcDateTime)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, PakistanTimeZone);
    }

    public async Task<PrintResponse> PrintDonorSlipsAsync(PrintDonorSlipsRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.FullName) ||
                string.IsNullOrWhiteSpace(request.NationalId) ||
                string.IsNullOrWhiteSpace(request.CouponCode))
            {
                return new PrintResponse
                {
                    Success = false,
                    Error = "Missing required fields"
                };
            }

            var currentDate = ConvertToPakistanTime(DateTime.UtcNow).ToString("dd/MM/yyyy");
            var currentTime = ConvertToPakistanTime(DateTime.UtcNow).ToString("hh:mm tt");

            await Task.Run(() =>
            {
                var e = new EPSON();
                
                // Collect all print commands into a byte array
                using var memoryStream = new MemoryStream();
                
                // Print Donor Copy
                var donorCopyBytes = PrintDonorCopyBytes(e, request, currentDate, currentTime);
                memoryStream.Write(donorCopyBytes, 0, donorCopyBytes.Length);
                
                // Print Lucky Draw Slip
                var luckyDrawBytes = PrintLuckyDrawSlipBytes(e, request, currentDate, currentTime);
                memoryStream.Write(luckyDrawBytes, 0, luckyDrawBytes.Length);
                
                // Print Office Copy
                var officeCopyBytes = PrintOfficeCopyBytes(e, request, currentDate, currentTime);
                memoryStream.Write(officeCopyBytes, 0, officeCopyBytes.Length);
                
                // Send all bytes to printer at once
                var allBytes = memoryStream.ToArray();
                bool success = WindowsRawPrinter.SendBytesToPrinter(_printerName, allBytes);
                
                if (!success)
                {
                    throw new Exception("Failed to send print job to printer");
                }
            });

            _logger.LogInformation("Successfully printed slips for donor: {FullName}", request.FullName);

            return new PrintResponse
            {
                Success = true,
                Message = "Slips printed successfully",
                CouponCode = request.CouponCode
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to print donor slips");
            return new PrintResponse
            {
                Success = false,
                Error = $"Print failed: {ex.Message}"
            };
        }
    }

    public async Task<PrintResponse> PrintTestPageAsync()
    {
        try
        {
            await Task.Run(() =>
            {
                var e = new EPSON();

                // Collect all print commands
                var commands = new List<byte[]>
                {
                    e.CenterAlign(),
                    e.SetStyles(PrintStyle.Bold | PrintStyle.Underline),
                    e.PrintLine("TEST PRINT"),
                    e.SetStyles(PrintStyle.None),
                    e.PrintLine(""),
                    e.PrintLine("Blood Connect Print Server"),
                    e.PrintLine("EPSON TM-T88V"),
                    e.PrintLine(""),
                    e.PrintLine($"Date: {DateTime.Now:dd/MM/yyyy}"),
                    e.PrintLine($"Time: {DateTime.Now:HH:mm:ss}"),
                    e.PrintLine(""),
                    e.PrintLine("If you can read this,"),
                    e.PrintLine("the printer is working correctly!"),
                    e.PrintLine(""),
                    e.PrintLine(""),
                    e.PartialCutAfterFeed(3)
                };

                // Combine all commands into one byte array
                var totalLength = commands.Sum(c => c.Length);
                var allBytes = new byte[totalLength];
                int offset = 0;
                foreach (var cmd in commands)
                {
                    Buffer.BlockCopy(cmd, 0, allBytes, offset, cmd.Length);
                    offset += cmd.Length;
                }

                // Send to printer
                bool success = WindowsRawPrinter.SendBytesToPrinter(_printerName, allBytes);
                
                if (!success)
                {
                    throw new Exception("Failed to send test print job to printer");
                }
            });

            return new PrintResponse
            {
                Success = true,
                Message = "Test page printed successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to print test page");
            return new PrintResponse
            {
                Success = false,
                Error = $"Test print failed: {ex.Message}"
            };
        }
    }

    private byte[] PrintDonorCopyBytes(EPSON e, PrintDonorSlipsRequest data, string date, string time)
    {
        var commands = new List<byte[]>
        {
            // Header
            e.CenterAlign(),
            e.SetStyles(PrintStyle.Bold),
            e.PrintLine("(1)"),
            e.PrintLine(""),
            e.SetStyles(PrintStyle.None),
            e.PrintLine("Sindh Blood Transfusion Authority (SBTA)"),
            e.PrintLine("Powered by Sunbonn"),
            e.PrintLine(""),

            // Slip Type
            e.SetStyles(PrintStyle.Bold),
            e.ReverseMode(true),
            e.PrintLine("   DONOR COPY   "),
            e.ReverseMode(false),
            e.SetStyles(PrintStyle.None),
            e.PrintLine(""),

            // Coupon Box
            e.PrintLine("================================"),
            e.SetStyles(PrintStyle.Bold),
            e.PrintLine("COUPON CODE"),
            e.SetStyles(PrintStyle.DoubleHeight | PrintStyle.DoubleWidth),
            e.PrintLine(data.CouponCode),
            e.SetStyles(PrintStyle.None),
            e.PrintLine("================================"),
            e.PrintLine(""),

            // Info
            e.LeftAlign(),
            e.PrintLine(FormatLineString("Name:", data.FullName)),
            e.PrintLine(FormatLineString("CNIC:", data.NationalId)),
            e.PrintLine(FormatLineString("Date:", date)),
            e.PrintLine(FormatLineString("Time:", time)),
            e.PrintLine(""),

            // Instructions
            e.PrintLine("Instructions:"),
            e.PrintLine("  * Keep this slip safe"),
            e.PrintLine("  * Present at screening station"),
            e.PrintLine("  * Show coupon code to staff"),
            e.PrintLine("  * Valid for today only"),
            e.PrintLine(""),

            // Footer
            e.CenterAlign(),
            e.PrintLine("Thank you for saving lives!"),
            e.PrintLine($"Printed: {date} {time}"),
            e.PrintLine(""),
            e.PrintLine(""),

            // Cut paper
            e.PartialCutAfterFeed(3)
        };

        return CombineBytes(commands);
    }

    private byte[] PrintLuckyDrawSlipBytes(EPSON e, PrintDonorSlipsRequest data, string date, string time)
    {
        var commands = new List<byte[]>
        {
            // Header
            e.CenterAlign(),
            e.SetStyles(PrintStyle.Bold),
            e.PrintLine("(2)"),
            e.PrintLine(""),
            e.SetStyles(PrintStyle.Bold | PrintStyle.Underline),
            e.PrintLine("LUCKY DRAW ENTRY"),
            e.SetStyles(PrintStyle.None),
            e.PrintLine("Sindh Blood Transfusion Authority (SBTA)"),
            e.PrintLine("Powered by Sunbonn"),
            e.PrintLine(""),

            // Slip Type 
            e.SetStyles(PrintStyle.Bold),
            e.ReverseMode(true),
            e.PrintLine("    COUPON CODE    "),
            e.ReverseMode(false),
            e.SetStyles(PrintStyle.None),
            e.PrintLine(""),

            // Entry Number
            e.PrintLine("================================"),
            e.SetStyles(PrintStyle.DoubleHeight | PrintStyle.DoubleWidth),
            e.PrintLine(data.CouponCode),
            e.SetStyles(PrintStyle.None),
            e.PrintLine("================================"),
            e.PrintLine(""),

            // Info
            e.LeftAlign(),
            e.PrintLine(FormatLineString("Name:", data.FullName)),
            e.PrintLine(FormatLineString("CNIC:", data.NationalId)),
            e.PrintLine(FormatLineString("Date:", date)),
            e.PrintLine(""),
            
            // Instructions
            e.PrintLine("Instructions:"),
            e.PrintLine("  * Put the coupon in the lucky draw"),
            e.PrintLine("  * Winner will be informed via call"),
            e.PrintLine(""),

            // Footer
            e.CenterAlign(),
            e.PrintLine("Good Luck!"),
            e.PrintLine($"Entry: {date} {time}"),
            e.PrintLine(""),
            e.PrintLine(""),

            // Cut paper
            e.PartialCutAfterFeed(3)
        };

        return CombineBytes(commands);
    }

    private byte[] PrintOfficeCopyBytes(EPSON e, PrintDonorSlipsRequest data, string date, string time)
    {
        var commands = new List<byte[]>
        {
            // Header
            e.CenterAlign(),
            e.SetStyles(PrintStyle.Bold),
            e.PrintLine("(3)"),
            e.PrintLine(""),
            e.SetStyles(PrintStyle.Bold | PrintStyle.Underline),
            e.PrintLine("SBTA INTERNAL"),
            e.SetStyles(PrintStyle.None),
            e.PrintLine("Sindh Blood Transfusion Authority (SBTA)"),
            e.PrintLine("Powered by Sunbonn"),
            e.PrintLine(""),

            // Slip Type
            e.SetStyles(PrintStyle.Bold),
            e.ReverseMode(true),
            e.PrintLine("    OFFICE COPY - FILE    "),
            e.ReverseMode(false),
            e.SetStyles(PrintStyle.None),
            e.PrintLine(""),

            // Donor ID
            e.PrintLine("================================"),
            e.SetStyles(PrintStyle.Bold),
            e.PrintLine("COUPON CODE"),
            e.SetStyles(PrintStyle.DoubleHeight | PrintStyle.DoubleWidth),
            e.PrintLine(data.CouponCode),
            e.SetStyles(PrintStyle.None),
            e.PrintLine("================================"),
            e.PrintLine(""),

            // Registration Info
            e.LeftAlign(),
            e.PrintLine(FormatLineString("Name:", data.FullName)),
            e.PrintLine(FormatLineString("CNIC:", data.NationalId)),
            e.PrintLine(FormatLineString("Reg. Date:", date)),
            e.PrintLine(FormatLineString("Reg. Time:", time)),
            e.PrintLine(""),
            e.PrintLine("--------------------------------"),
            e.PrintLine(""),

            // Footer
            e.CenterAlign(),
            e.SetStyles(PrintStyle.Bold),
            e.PrintLine("SBTA USE ONLY"),
            e.SetStyles(PrintStyle.None),
            e.PrintLine($"Printed: {date} {time}"),
            e.PrintLine(""),

            // Final cut
            e.PartialCutAfterFeed(5)
        };

        return CombineBytes(commands);
    }

    private string FormatLineString(string label, string value, int width = 48)
    {
        var dots = new string('.', Math.Max(0, width - label.Length - value.Length));
        return $"{label}:..........{value}";
    }

    private byte[] CombineBytes(List<byte[]> arrays)
    {
        var totalLength = arrays.Sum(a => a.Length);
        var result = new byte[totalLength];
        int offset = 0;
        
        foreach (var array in arrays)
        {
            Buffer.BlockCopy(array, 0, result, offset, array.Length);
            offset += array.Length;
        }
        
        return result;
    }
}
