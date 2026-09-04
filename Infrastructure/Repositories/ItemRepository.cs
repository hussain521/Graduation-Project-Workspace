namespace Infrastructure.Repositories
{
    public class ItemRepository : GenericRepository<Item>
    {
        public ItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<Result<List<Item>>> GetAllAsync()
        {
            try
            {
                var QueryResult = await _dbSet
                    .Include(s => s.User)
                    .Include(i=>i.Currency)
                    .ToListAsync();
                return Result.Success<List<Item>?>(QueryResult);
            }
            catch (Exception ex)
            {
                return Result.Failure<List<Item>>(new Error(ex.Message));
            }
        }
    }
}