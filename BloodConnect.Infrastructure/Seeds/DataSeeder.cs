using BloodConnect.Core.Entities;
using BloodConnect.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodConnect.Infrastructure.Seeds;

public static class DataSeeder
{
    public static async Task SeedAsync(BloodConnectDbContext context)
    {
        await context.Database.MigrateAsync();

        // Seed Branches
        if (!await context.Branches.AnyAsync())
        {
            var branches = new[]
            {
                new Branch
                {
                    BranchId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Name = "Downtown Blood Center",
                    Address = "123 Main St, Metro City",
                    IsActive = true
                },
                new Branch
                {
                    BranchId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    Name = "Westside Community Clinic",
                    Address = "456 Oak Ave, Westville",
                    IsActive = true
                },
                new Branch
                {
                    BranchId = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                    Name = "Northgate Medical Hub",
                    Address = "789 Pine Rd, Northgate",
                    IsActive = true
                },
                new Branch
                {
                    BranchId = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                    Name = "Southpark Health Station",
                    Address = "321 Elm Blvd, Southpark",
                    IsActive = true
                }
            };
            await context.Branches.AddRangeAsync(branches);
        }

        // Seed Deferral Reasons
        if (!await context.DeferralReasons.AnyAsync())
        {
            var deferralReasons = new[]
            {
                new DeferralReason
                {
                    DeferralReasonId = Guid.Parse("00000000-0000-0000-0001-000000000001"),
                    Code = "LOW_HB",
                    Label = "Low Hemoglobin",
                    Category = "Medical",
                    DefaultDurationDays = 90
                },
                new DeferralReason
                {
                    DeferralReasonId = Guid.Parse("00000000-0000-0000-0001-000000000002"),
                    Code = "HIGH_BP",
                    Label = "High Blood Pressure",
                    Category = "Medical",
                    DefaultDurationDays = 30
                },
                new DeferralReason
                {
                    DeferralReasonId = Guid.Parse("00000000-0000-0000-0001-000000000003"),
                    Code = "RECENT_TATTOO",
                    Label = "Recent Tattoo/Piercing",
                    Category = "Lifestyle",
                    DefaultDurationDays = 365
                },
                new DeferralReason
                {
                    DeferralReasonId = Guid.Parse("00000000-0000-0000-0001-000000000004"),
                    Code = "TRAVEL",
                    Label = "Recent Travel to Risk Area",
                    Category = "Travel",
                    DefaultDurationDays = 180
                },
                new DeferralReason
                {
                    DeferralReasonId = Guid.Parse("00000000-0000-0000-0001-000000000005"),
                    Code = "MEDICATION",
                    Label = "Currently on Medication",
                    Category = "Medical",
                    DefaultDurationDays = 30
                },
                new DeferralReason
                {
                    DeferralReasonId = Guid.Parse("00000000-0000-0000-0001-000000000006"),
                    Code = "RECENT_SURGERY",
                    Label = "Recent Surgery",
                    Category = "Medical",
                    DefaultDurationDays = 180
                },
                new DeferralReason
                {
                    DeferralReasonId = Guid.Parse("00000000-0000-0000-0001-000000000007"),
                    Code = "COLD_FLU",
                    Label = "Active Cold/Flu Symptoms",
                    Category = "Medical",
                    DefaultDurationDays = 14
                }
            };
            await context.DeferralReasons.AddRangeAsync(deferralReasons);
        }

        // Seed Sample Donors
        if (!await context.Donors.AnyAsync())
        {
            var donors = new[]
            {
                new Donor
                {
                    DonorId = Guid.Parse("00000000-0000-0000-0002-000000000001"),
                    FullName = "Sarah Johnson",
                    DateOfBirth = new DateTime(1990, 5, 15),
                    Gender = "female",
                    Phone = "+1-555-0101",
                    Email = "sarah.j@example.com",
                    NationalId = "NID-1234567",
                    Address = new Address
                    {
                        Line1 = "100 Maple St",
                        City = "Metro City",
                        Province = "State",
                        Country = "US",
                        PostalCode = "10001"
                    },
                    CouponCode = "BLOOD2024",
                    CreatedAt = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 6, 20, 14, 30, 0, DateTimeKind.Utc),
                    LastDonationDate = new DateTime(2024, 6, 20)
                },
                new Donor
                {
                    DonorId = Guid.Parse("00000000-0000-0000-0002-000000000002"),
                    FullName = "Michael Chen",
                    DateOfBirth = new DateTime(1985, 11, 22),
                    Gender = "male",
                    Phone = "+1-555-0202",
                    Email = "mchen@example.com",
                    NationalId = "NID-7654321",
                    Address = new Address
                    {
                        Line1 = "200 Cedar Ave",
                        City = "Westville",
                        Province = "State",
                        Country = "US",
                        PostalCode = "20002"
                    },
                    CouponCode = "GIVE2024",
                    CreatedAt = new DateTime(2024, 3, 10, 9, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 3, 10, 9, 0, 0, DateTimeKind.Utc)
                }
            };
            await context.Donors.AddRangeAsync(donors);
        }

        await context.SaveChangesAsync();
    }
}

