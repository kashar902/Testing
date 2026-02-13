namespace BloodConnect.Core.DTOs;

public class PrintDonorSlipsRequest
{
    public string FullName { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public string CouponCode { get; set; } = string.Empty;
}

public class PrintResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? Error { get; set; }
    public string? CouponCode { get; set; }
}

public class PrinterInfo
{
    public string Name { get; set; } = string.Empty;
    public string VendorId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public bool IsConnected { get; set; }
}
