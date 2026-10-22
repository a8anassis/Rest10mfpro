using Microsoft.EntityFrameworkCore;
using SchoolApp.Data;

namespace SchoolApp.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly SchoolMvcDbfirstContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(SchoolMvcDbfirstContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();      // dynamically retrieves DbSet
        }

        public virtual async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);


        public virtual async Task AddRangeAsync(IEnumerable<T> entities) => await _dbSet.AddRangeAsync(entities);

        public virtual Task UpdateAsync(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }


        public virtual async Task<bool> DeleteAsync(int id)
        {
            T? existingEntity = await _dbSet.FindAsync(id);
            if (existingEntity is null) return false;
            _dbSet.Remove(existingEntity);
            return true;
        }

        public virtual async Task<T?> GetAsync(int id) => await _dbSet.FindAsync(id);


        public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();


        public virtual async Task<int> GetCountAsync() => await _dbSet.CountAsync();
    }
}
