using Document_Intelligence_Task.Data;
using Document_Intelligence_Task.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Document_Intelligence_Task.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DocumentIntelligenceDB _dbContext;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(DocumentIntelligenceDB dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }
        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
        public void DeleteAsync(T entity) => _dbSet.Remove(entity);
        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
        public async Task<T?> GetByIdAsync<Tin>(Tin id) => await _dbSet.FindAsync(id);
        
    }
}
