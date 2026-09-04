namespace Infrastructure.Repositories.Base
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<Result<List<T>>> GetAllAsync()
        {
            try
            {
                var QueryResult = await _dbSet.ToListAsync();
                return Result.Success<List<T>?>(QueryResult);
            }
            catch (Exception ex)
            {
                return Result.Failure<List<T>>(new Error(ex.Message));
            }
        }

        public virtual async Task<Result<List<T>>> GetListAsync()
        {
            try
            {
                var QueryResult = await _dbSet.ToListAsync();
                return Result.Success<List<T>?>(QueryResult);
            }
            catch (Exception ex)
            {
                return Result.Failure<List<T>>(new Error(ex.Message));
            }
        }

        public virtual async Task<Result<T>> FindByIdAsync(Guid id)
        {
            try
            {
                var QueryResult = await _dbSet.FindAsync(id);
                return Result.Success<T?>(QueryResult);
            }
            catch (Exception ex)
            {
                return Result.Failure<T>(new Error(ex.Message));
            }
        }

        public virtual async Task<Result<T>> AddAsync(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
                return Result.Success<T?>(entity);
            }
            catch (Exception ex)
            {
                return Result.Failure<T>(new Error(ex.Message));
            }
        }

        public virtual async Task<Result<T>> UpdateAsync(Guid id, T entity)
        {
            try
            {
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
                return Result.Success<T?>(entity);
            }
            catch (Exception ex)
            {
                return Result.Failure<T>(new Error(ex.Message));
            }
        }

        public virtual async Task<Result<T>> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                    await _context.SaveChangesAsync();
                }
                return Result.Success<T?>(entity);
            }
            catch (Exception ex)
            {
                return Result.Failure<T>(new Error(ex.Message));
            }
        }
    }
}