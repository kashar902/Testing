namespace BloodConnect.Core.DTOs;

public class DeferralReasonResponse
{
    public Guid DeferralReasonId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int DefaultDurationDays { get; set; }
}

