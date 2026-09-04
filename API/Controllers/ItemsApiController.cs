using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.UnifiedResult;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ItemsApiController : BaseController<Item>
    {
        private readonly ItemRepository _repository;

        public ItemsApiController(ItemRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ActionName("GetAll")]
        [Authorize(Roles = Shared.Constant.Roles.Items.Page)]
        public async Task<Result<List<Item>>> GetAll()
        {
            var userInfo = GetUserInfo();

            var result = await _repository.GetAllAsync();

            if (!result.IsSuccess)
            {
                return result;
            }

            if (userInfo != null && userInfo.OrganizationId.HasValue)
            {
                var items = result.Data?
                    .Where(i => i.OrganizationId == userInfo.OrganizationId.Value)
                    .ToList()
                    ?? new List<Item>();

                return new Result<List<Item>>(
                    result.IsSuccess,
                    result.Error,
                    items
                );
            }

            return result;
        }

        [HttpGet]
        [ActionName("GetList")]
        [Authorize]
        public async Task<Result<List<Item>>> GetList()
        {
            var userInfo = GetUserInfo();

            var result = await _repository.GetListAsync();

            if (!result.IsSuccess)
            {
                return result;
            }

            if (userInfo != null && userInfo.OrganizationId.HasValue)
            {
                var items = result.Data?
                    .Where(i => i.OrganizationId == userInfo.OrganizationId.Value)
                    .ToList()
                    ?? new List<Item>();

                return new Result<List<Item>>(
                    result.IsSuccess,
                    result.Error,
                    items
                );
            }

            return result;
        }

        [HttpGet("{id}")]
        [ActionName("FindById")]
        [Authorize]
        public async Task<Result<Item>> FindById(Guid id)
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
                return Result.Failure<Item>(
                    new Shared.UnifiedResult.Error(
                        "غير مصرح بالوصول لهذا الصنف"
                    )
                );
            }

            return result;
        }

        [HttpPost]
        [ActionName("Add")]
        [Authorize(Roles = Shared.Constant.Roles.Items.Add)]
        public async Task<Result<Item>> Add([FromBody] Item item)
        {
            item = AddBaseInfo(item);

            var result = await _repository.AddAsync(item);

            return result;
        }

        [HttpPut("{id}")]
        [ActionName("Update")]
        [Authorize(Roles = Shared.Constant.Roles.Items.Update)]
        public async Task<Result<Item>> Update(
            Guid id,
            [FromBody] Item item)
        {
            var userInfo = GetUserInfo();

            if (userInfo != null && userInfo.OrganizationId.HasValue)
            {
                item.OrganizationId = userInfo.OrganizationId;
            }

            var result = await _repository.UpdateAsync(id, item);

            return result;
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Shared.Constant.Roles.Items.Delete)]
        public async Task<Result<Item>> Delete(Guid id)
        {
            var result = await _repository.DeleteAsync(id);

            return result;
        }
    }
}