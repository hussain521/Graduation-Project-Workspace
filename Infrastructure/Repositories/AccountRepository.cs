namespace Infrastructure.Repositories
{
    public class AccountRepository : GenericRepository<Account>
    {
        public AccountRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<Result<Account>> AddAsync(Account entity)
        {
            var account = _context.Accounts.Where(a=>a.Name == entity.Name).FirstOrDefault();
            if (account == null)
            {
                var AddResult = await base.AddAsync(entity);
                if(AddResult!=null && AddResult.IsSuccess)
                {
                    var x = this._dbSet.Include(s => s.User).Include(a=>a.Category)
                        .Where(a => a.Id == AddResult.Data.Id).FirstOrDefault();
                    return Result.Success(x);
                }
                return AddResult;
            }
            else
            {
                return Result.Failure<Account>(new Error("الحساب موجود فعلاً"));
            }
        }

        public override async Task<Result<List<Account>>> GetAllAsync()
        {
            try
            {
                var QueryResult = await _dbSet.Include(s => s.User).Include(a=>a.Category).ToListAsync();
                return Result.Success<List<Account>?>(QueryResult);
            }
            catch (Exception ex)
            {
                return Result.Failure<List<Account>>(new Error(ex.Message));
            }
        }
    }
}