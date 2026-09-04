namespace Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<Result<List<Category>>> GetAllAsync()
        {
            try
            {
                var QueryResult = await _dbSet.Include(s => s.User).ToListAsync();
                return Result.Success<List<Category>?>(QueryResult);
            }
            catch (Exception ex)
            {
                return Result.Failure<List<Category>>(new Error(ex.Message));
            }
        }
    }
}
