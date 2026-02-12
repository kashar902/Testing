using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodConnect.Core.Entities;

public class DonationScreening
{
    [Key]
    public Guid ScreeningId { get; set; }
    
    [Required]
    public Guid DonorId { get; set; }
    
    [Required]
    public Guid BranchId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string StaffId { get; set; } = string.Empty;
    
    public Vitals Vitals { get; set; } = new();
    
    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string EligibilityStatus { get; set; } = string.Empty; // "eligible" or "deferred"
    
    public Guid? DeferralReasonId { get; set; }
    public DateTime? DeferralUntil { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("DonorId")]
    public Donor Donor { get; set; } = null!;
    
    [ForeignKey("BranchId")]
    public Branch Branch { get; set; } = null!;
    
    [ForeignKey("DeferralReasonId")]
    public DeferralReason? DeferralReason { get; set; }
}

