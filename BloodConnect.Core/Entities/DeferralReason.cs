using System.ComponentModel.DataAnnotations;

namespace BloodConnect.Core.Entities;

public class DeferralReason
{
    [Key]
    public Guid DeferralReasonId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string Label { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;
    
    public int DefaultDurationDays { get; set; }
    
    // Navigation properties
    public ICollection<DonationScreening> Screenings { get; set; } = new List<DonationScreening>();
}

