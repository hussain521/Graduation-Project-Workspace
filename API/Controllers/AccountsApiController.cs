using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.UnifiedResult;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountsApiController : BaseController<Account>
    {
        private readonly AccountRepository _repository;

        public AccountsApiController(AccountRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ActionName("GetAll")]
        [Authorize(Roles = Shared.Constant.Roles.Accounts.Page)]
        public async Task<Result<List<Account>>> GetAll()
        {
            var userInfo = GetUserInfo();

            var result = await _repository.GetAllAsync();

            if (!result.IsSuccess)
            {
                return result;
            }

            if (userInfo != null && userInfo.OrganizationId.HasValue)
            {
                var accounts = result.Data?
                    .Where(a => a.OrganizationId == userInfo.OrganizationId.Value)
                    .ToList()
                    ?? new List<Account>();

                return new Result<List<Account>>(
                    result.IsSuccess,
                    result.Error,
                    accounts
                );
            }

            return result;
        }

        [HttpGet]
        [ActionName("GetList")]
        [Authorize]
        public async Task<Result<List<Account>>> GetList()
        {
            var userInfo = GetUserInfo();

            var result = await _repository.GetListAsync();

            if (!result.IsSuccess)
            {
                return result;
            }

            if (userInfo != null && userInfo.OrganizationId.HasValue)
            {
                var accounts = result.Data?
                    .Where(a => a.OrganizationId == userInfo.OrganizationId.Value)
                    .ToList()
                    ?? new List<Account>();

                return new Result<List<Account>>(
                    result.IsSuccess,
                    result.Error,
                    accounts
                );
            }

            return result;
        }

        [HttpGet("{id}")]
        [ActionName("FindById")]
        [Authorize]
        public async Task<Result<Account>> FindById(Guid id)
        {
            var userInfo = GetUserInfo();

            var result = await _repository.FindByIdAsync(id);

            if (!result.IsSuccess)
            {
                return result;
            }

            if (
                result.Data != null &&
                userInfo != null &&
                userInfo.OrganizationId.HasValue
            )
            {
                if (result.Data.OrganizationId != userInfo.OrganizationId.Value)
                {
                    return Result.Failure<Account>(
                        new Shared.UnifiedResult.Error(
                            "غير مصرح بالوصول لهذا الحساب"
                        )
                    );
                }
            }

            return result;
        }

        [HttpPost]
        [ActionName("Add")]
        [Authorize(Roles = Shared.Constant.Roles.Accounts.Add)]
        public async Task<Result<Account>> Add(
            [FromBody] Account account)
        {
            account = AddBaseInfo(account);

            var result = await _repository.AddAsync(account);

            return result;
        }

        [HttpPut("{id}")]
        [ActionName("Update")]
        [Authorize(Roles = Shared.Constant.Roles.Accounts.Update)]
        public async Task<Result<Account>> Update(
            Guid id,
            [FromBody] Account account)
        {
            var userInfo = GetUserInfo();

            if (userInfo != null && userInfo.OrganizationId.HasValue)
            {
                account.OrganizationId = userInfo.OrganizationId;
            }

            var result = await _repository.UpdateAsync(id, account);

            return result;
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Shared.Constant.Roles.Accounts.Delete)]
        public async Task<Result<Account>> Delete(Guid id)
        {
            var result = await _repository.DeleteAsync(id);

            return result;
        }
    }
}