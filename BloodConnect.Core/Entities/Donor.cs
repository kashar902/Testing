using System.ComponentModel.DataAnnotations;

namespace BloodConnect.Core.Entities;

public class Donor
{
    [Key]
    public Guid DonorId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string? FatherHusbandName { get; set; }
    
    [Required]
    public int Age { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Gender { get; set; } = string.Empty; // "male", "female", "other"
    
    [MaxLength(10)]
    public string? BloodGroup { get; set; } // "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-"
    
    [Required]
    [MaxLength(50)]
    public string Phone { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string NationalId { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? District { get; set; }
    
    public Address Address { get; set; } = new();
    
    public int? TimesDonatedBefore { get; set; }
    
    [MaxLength(500)]
    public string? SourceOfInfo { get; set; }
    
    [Required]
    [MaxLength(12)]
    public string CouponCode { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastDonationDate { get; set; }
    
    // Navigation properties
    public ICollection<DonationScreening> Screenings { get; set; } = new List<DonationScreening>();
}

