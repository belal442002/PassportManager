using Microsoft.EntityFrameworkCore;

namespace Document_Intelligence_Task.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync<Tin>(Tin id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        void DeleteAsync(T entity);
    }
}
