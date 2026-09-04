namespace Infrastructure.Repositories.Base
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<Result<List<T>>> GetAllAsync();
        Task<Result<T>> FindByIdAsync(Guid id);
        Task<Result<T>> AddAsync(T product);
        Task<Result<T>> UpdateAsync(Guid id, T entity);
        Task<Result<T>> DeleteAsync(Guid id);
    }
}
