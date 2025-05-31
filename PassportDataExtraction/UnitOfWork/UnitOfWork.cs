using Document_Intelligence_Task.Data;
using Document_Intelligence_Task.Interfaces;
using Document_Intelligence_Task.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Document_Intelligence_Task.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DocumentIntelligenceDB _dbContext;
        private IDbContextTransaction? _transaction;

        public IPassportRepository Passports { get; private set; }
        public UnitOfWork(DocumentIntelligenceDB dbContext)
        {
            _dbContext = dbContext;
            Passports = new PassportRepository(_dbContext);
        }

        public async Task  BeginTransactionAsync()
        {
            if(_transaction != null)
            {
                throw new InvalidOperationException("A transaction is already in progress.");
            }
            _transaction = await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction to commit.");
            }
            await _transaction.CommitAsync();
            _transaction = null;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public async Task RollBackAsync()
        {
            if(_transaction == null)
            {
                throw new InvalidOperationException("No transaction to roll back.");
            }
            await _transaction.RollbackAsync();
            _transaction = null;
        }

        public async Task<bool> CompleteAsync()
        {
            try
            {
                await SaveChangesAsync();
                if(_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
                return true;
            }catch (Exception)
            {
                if(_transaction!= null)
                {
                    await _transaction.RollbackAsync();
                }
                return false;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _dbContext.Dispose(); 
        }
    }
}
