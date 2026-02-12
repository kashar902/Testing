namespace BloodConnect.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IDonorRepository Donors { get; }
    IScreeningRepository Screenings { get; }
    IBranchRepository Branches { get; }
    IDeferralReasonRepository DeferralReasons { get; }
    IUserRepository Users { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

