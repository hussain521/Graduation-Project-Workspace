using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.UnifiedResult;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CurrenciesApiController : BaseController<Currency>
    {
        private readonly CurrencyRepository _repository;

        public CurrenciesApiController(CurrencyRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ActionName("GetAll")]
        [Authorize(Roles = Shared.Constant.Roles.Currencies.Page)]
        public async Task<Result<List<Currency>>> GetAll()
        {
            var userInfo = GetUserInfo();

            var result = await _repository.GetAllAsync();

            if (!result.IsSuccess)
            {
                return result;
            }

            if (userInfo != null && userInfo.OrganizationId.HasValue)
            {
                var currencies = result.Data?
                    .Where(c => c.OrganizationId == userInfo.OrganizationId.Value)
                    .ToList()
                    ?? new List<Currency>();

                return new Result<List<Currency>>(
                    result.IsSuccess,
                    result.Error,
                    currencies
                );
            }

            return result;
        }

        [HttpGet]
        [ActionName("GetList")]
        [Authorize]
        public async Task<Result<List<Currency>>> GetList()
        {
            var userInfo = GetUserInfo();

            var result = await _repository.GetListAsync();

            if (!result.IsSuccess)
            {
                return result;
            }

            if (userInfo != null && userInfo.OrganizationId.HasValue)
            {
                var currencies = result.Data?
                    .Where(c => c.OrganizationId == userInfo.OrganizationId.Value)
                    .ToList()
                    ?? new List<Currency>();

                return new Result<List<Currency>>(
                    result.IsSuccess,
                    result.Error,
                    currencies
                );
            }

            return result;
        }

        [HttpGet("{id}")]
        [ActionName("FindById")]
        [Authorize]
        public async Task<Result<Currency>> FindById(Guid id)
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
                userInfo.OrganizationId.HasValue &&
                result.Data.OrganizationId != userInfo.OrganizationId.Value
            )
            {
                return Result.Failure<Currency>(
                    new Shared.UnifiedResult.Error(
                        "غير مصرح بالوصول لهذه العملة"
                    )
                );
            }

            return result;
        }

        [HttpPost]
        [ActionName("Add")]
        [Authorize(Roles = Shared.Constant.Roles.Currencies.Add)]
        public async Task<Result<Currency>> Add(
            [FromBody] Currency currency)
        {
            currency = AddBaseInfo(currency);

            var result = await _repository.AddAsync(currency);

            return result;
        }

        [HttpPut("{id}")]
        [ActionName("Update")]
        [Authorize(Roles = Shared.Constant.Roles.Currencies.Update)]
        public async Task<Result<Currency>> Update(
            Guid id,
            [FromBody] Currency currency)
        {
            var userInfo = GetUserInfo();

            if (userInfo != null && userInfo.OrganizationId.HasValue)
            {
                currency.OrganizationId = userInfo.OrganizationId;
            }

            var result = await _repository.UpdateAsync(id, currency);

            return result;
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Shared.Constant.Roles.Currencies.Delete)]
        public async Task<Result<Currency>> Delete(Guid id)
        {
            var result = await _repository.DeleteAsync(id);

            return result;
        }
    }
}