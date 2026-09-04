namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountsApiController : BaseController<Account>
    {
        AccountRepository _repository;
        public AccountsApiController(AccountRepository repository)
        {
            this._repository = repository;
        }
        
        [HttpGet]
        [ActionName("GetAll")]
        [Authorize(Roles = Shared.Constant.Roles.Accounts.Page)]
        public async Task<Result<List<Account>>> GetAll()
        {
            var userInfo = this.GetUserInfo();
            var Result = await this._repository.GetAllAsync();
            if (Result.IsSuccess && userInfo != null && userInfo.OrganizationId.HasValue)
            {
                Result.Data = Result.Data.Where(a => a.OrganizationId == userInfo.OrganizationId).ToList();
            }
            return Result;
        }

        [HttpGet]
        [ActionName("GetList")]
        [Authorize]
        public async Task<Result<List<Account>>> GetList()
        {
            var userInfo = this.GetUserInfo();
            var Result = await this._repository.GetListAsync();
            if (Result.IsSuccess && userInfo != null && userInfo.OrganizationId.HasValue)
            {
                Result.Data = Result.Data.Where(a => a.OrganizationId == userInfo.OrganizationId).ToList();
            }
            return Result;
        }

        [HttpGet("{id}")]
        [ActionName("FindById")]
        [Authorize]
        public async Task<Result<Account>> FindById(Guid id)
        {
            var userInfo = this.GetUserInfo();
            var Result = await this._repository.FindByIdAsync(id);
            if (Result.IsSuccess && Result.Data != null && userInfo != null && userInfo.OrganizationId.HasValue)
            {
                if (Result.Data.OrganizationId != userInfo.OrganizationId)
                {
                    return Result.Failure<Account>(new Shared.Constant.Roles.Error("غير مصرح بالوصول لهذا الحساب"));
                }
            }
            return Result;
        }
        
        [HttpPost]
        [ActionName("Add")]
        [Authorize(Roles =Shared.Constant.Roles.Accounts.Add)]
        public async Task<Result<Account>> Add([FromBody] Account account)
        {
            account = this.AddBaseInfo(account);
            var Result = await this._repository.AddAsync(account);
            return Result;
        }
        
        [HttpPut("{id}")]
        [ActionName("Update")]
        [Authorize(Roles = Shared.Constant.Roles.Accounts.Update)]
        public async Task<Result<Account>> Update(Guid id, [FromBody] Account account)
        {
            var userInfo = this.GetUserInfo();
            if (userInfo != null && userInfo.OrganizationId.HasValue)
            {
                account.OrganizationId = userInfo.OrganizationId;
            }
            var Result = await this._repository.UpdateAsync(id, account);
            return Result;
        }
        
        [HttpDelete("{id}")]
        [Authorize(Roles = Shared.Constant.Roles.Accounts.Delete)]
        public async Task<Result<Account>> Delete(Guid id)
        {
            var Result = await this._repository.DeleteAsync(id);
            return Result;
        }
    }
}