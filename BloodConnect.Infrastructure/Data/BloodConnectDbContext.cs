using BloodConnect.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace BloodConnect.Infrastructure.Data;

public class BloodConnectDbContext : DbContext
{
    public BloodConnectDbContext(DbContextOptions<BloodConnectDbContext> options)
        : base(options)
    {
    }

    public DbSet<Donor> Donors => Set<Donor>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<DeferralReason> DeferralReasons => Set<DeferralReason>();
    public DbSet<DonationScreening> DonationScreenings => Set<DonationScreening>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Donor configuration
        modelBuilder.Entity<Donor>(entity =>
        {
            entity.HasKey(e => e.DonorId);
            entity.Property(e => e.DonorId).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.CouponCode).IsUnique();
            entity.HasIndex(e => e.NationalId).IsUnique();
            
            // Configure Address as owned type
            entity.OwnsOne(e => e.Address, address =>
            {
                address.Property(a => a.Line1).HasMaxLength(500);
                address.Property(a => a.City).HasMaxLength(100);
                address.Property(a => a.Province).HasMaxLength(100);
                address.Property(a => a.Country).HasMaxLength(100);
                address.Property(a => a.PostalCode).HasMaxLength(20);
            });
        });

        // Branch configuration
        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.BranchId);
            entity.Property(e => e.BranchId).ValueGeneratedOnAdd();
        });

        // DeferralReason configuration
        modelBuilder.Entity<DeferralReason>(entity =>
        {
            entity.HasKey(e => e.DeferralReasonId);
            entity.Property(e => e.DeferralReasonId).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // DonationScreening configuration
        modelBuilder.Entity<DonationScreening>(entity =>
        {
            entity.HasKey(e => e.ScreeningId);
            entity.Property(e => e.ScreeningId).ValueGeneratedOnAdd();
            
            // Configure Vitals as owned type
            entity.OwnsOne(e => e.Vitals, vitals =>
            {
                vitals.Property(v => v.TempC).HasPrecision(4, 2);
                vitals.Property(v => v.WeightKg).HasPrecision(5, 2);
                vitals.Property(v => v.HbGdl).HasPrecision(4, 2);
            });

            // Relationships
            entity.HasOne(e => e.Donor)
                .WithMany(d => d.Screenings)
                .HasForeignKey(e => e.DonorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Branch)
                .WithMany(b => b.Screenings)
                .HasForeignKey(e => e.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.DeferralReason)
                .WithMany(dr => dr.Screenings)
                .HasForeignKey(e => e.DeferralReasonId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });
    }
}

