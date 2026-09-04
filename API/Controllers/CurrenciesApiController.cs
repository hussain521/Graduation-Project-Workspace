namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CurrenciesApiController : BaseController<Currency>
    {
        CurrencyRepository _repository;
        public CurrenciesApiController(CurrencyRepository repository)
        {
            this._repository = repository;
        }

        // GET: api/<CurrenciesApiController>
        [HttpGet]
        [ActionName("GetAll")]
        [Authorize(Roles = Shared.Constant.Roles.Currencies.Page)]
        public async Task<Result<List<Currency>>> GetAll()
        {
            var userInfo = this.GetUserInfo();
            var Result = await this._repository.GetAllAsync();
            if (Result.IsSuccess && userInfo != null && userInfo.OrganizationId.HasValue)
            {
                Result.Data = Result.Data.Where(c => c.OrganizationId == userInfo.OrganizationId).ToList();
            }
            return Result;
        }

        [HttpGet]
        [ActionName("GetList")]
        [Authorize]
        public async Task<Result<List<Currency>>> GetList()
        {
            var userInfo = this.GetUserInfo();
            var Result = await this._repository.GetListAsync();
            if (Result.IsSuccess && userInfo != null && userInfo.OrganizationId.HasValue)
            {
                Result.Data = Result.Data.Where(c => c.OrganizationId == userInfo.OrganizationId).ToList();
            }
            return Result;
        }

        // GET api/<CurrenciesApiController>/5
        [HttpGet("{id}")]
        [ActionName("FindById")]
        [Authorize]
        public async Task<Result<Currency>> FindById(Guid id)
        {
            var userInfo = this.GetUserInfo();
            var Result = await this._repository.FindByIdAsync(id);
            if (Result.IsSuccess && Result.Data != null && userInfo != null && userInfo.OrganizationId.HasValue)
            {
                if (Result.Data.OrganizationId != userInfo.OrganizationId)
                {
                    return Result.Failure<Currency>(new Shared.Constant.Roles.Error("غير مصرح بالوصول لهذه العملة"));
                }
            }
            return Result;
        }

        // POST api/<CurrenciesApiController>
        [HttpPost]
        [ActionName("Add")]
        [Authorize(Roles = Shared.Constant.Roles.Currencies.Add)]
        public async Task<Result<Currency>> Add([FromBody] Currency currency)
        {
            currency = this.AddBaseInfo(currency);
            var Result = await this._repository.AddAsync(currency);
            return Result;
        }

        // PUT api/<CurrenciesApiController>/5
        [HttpPut("{id}")]
        [ActionName("Update")]
        [Authorize(Roles = Shared.Constant.Roles.Currencies.Update)]
        public async Task<Result<Currency>> Update(Guid id, [FromBody] Currency currency)
        {
            var userInfo = this.GetUserInfo();
            if (userInfo != null && userInfo.OrganizationId.HasValue)
            {
                currency.OrganizationId = userInfo.OrganizationId;
            }
            var Result = await this._repository.UpdateAsync(id, currency);
            return Result;
        }

        // DELETE api/<CurrenciesApiController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = Shared.Constant.Roles.Currencies.Delete)]
        public async Task<Result<Currency>> Delete(Guid id)
        {
            var Result = await this._repository.DeleteAsync(id);
            return Result;
        }
    }
}