namespace Infrastructure.Repositories
{
    public class CurrencyRepository : GenericRepository<Currency>
    {
        public CurrencyRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<Result<List<Currency>>> GetAllAsync()
        {
            try
            {
                var QueryResult = await _dbSet.Include(s => s.User).ToListAsync();
                return Result.Success<List<Currency>?>(QueryResult);
            }
            catch (Exception ex)
            {
                return Result.Failure<List<Currency>>(new Error(ex.Message));
            }
        }
    }
}