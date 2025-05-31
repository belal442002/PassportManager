using Document_Intelligence_Task.Interfaces;

namespace Document_Intelligence_Task.UOW
{
    public interface IUnitOfWork : IDisposable
    {
        // Repositories
        IPassportRepository Passports { get; }

        //Methods
        Task BeginTransactionAsync();
        Task<int> SaveChangesAsync();
        Task CommitAsync();
        Task RollBackAsync();
        Task<bool> CompleteAsync();
    }
}
