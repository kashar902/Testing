using System.ComponentModel.DataAnnotations;

namespace BloodConnect.Core.Entities;

public class Branch
{
    [Key]
    public Guid BranchId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public ICollection<DonationScreening> Screenings { get; set; } = new List<DonationScreening>();
}

