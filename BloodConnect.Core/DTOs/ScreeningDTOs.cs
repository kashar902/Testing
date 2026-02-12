namespace BloodConnect.Core.DTOs;

public class CreateScreeningRequest
{
    public Guid DonorId { get; set; }
    public Guid BranchId { get; set; }
    public string StaffId { get; set; } = string.Empty;
    public VitalsDto Vitals { get; set; } = new();
    public string Notes { get; set; } = string.Empty;
    public string EligibilityStatus { get; set; } = string.Empty; // "eligible" or "deferred"
    public Guid? DeferralReasonId { get; set; }
    public string? DeferralUntil { get; set; }
}

public class ScreeningResponse
{
    public Guid ScreeningId { get; set; }
    public Guid DonorId { get; set; }
    public Guid BranchId { get; set; }
    public string StaffId { get; set; } = string.Empty;
    public VitalsDto Vitals { get; set; } = new();
    public string Notes { get; set; } = string.Empty;
    public string EligibilityStatus { get; set; } = string.Empty;
    public Guid? DeferralReasonId { get; set; }
    public string? DeferralUntil { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class VitalsDto
{
    public int BpSystolic { get; set; }
    public int BpDiastolic { get; set; }
    public int Pulse { get; set; }
    public decimal TempC { get; set; }
    public decimal WeightKg { get; set; }
    public decimal HbGdl { get; set; }
}

