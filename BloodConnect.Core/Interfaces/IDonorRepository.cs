using BloodConnect.Core.Entities;

namespace BloodConnect.Core.Interfaces;

public interface IDonorRepository : IRepository<Donor>
{
    Task<Donor?> GetByCouponCodeAsync(string couponCode);
    Task<bool> ExistsByCouponCodeAsync(string couponCode);
    Task<bool> ExistsByNationalIdAsync(string nationalId);
    Task<bool> ExistsByPhoneAndDobAsync(string phone, DateTime dob);
    Task<IEnumerable<DonationScreening>> GetDonorScreeningsAsync(Guid donorId);
    Task<string> GetMaxCouponCodeAsync();
}

