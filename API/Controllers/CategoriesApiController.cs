using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.UnifiedResult;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoriesApiController : BaseController<Category>
    {
        private readonly CategoryRepository _repository;

        public CategoriesApiController(CategoryRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ActionName("GetAll")]
        [Authorize(Roles = Shared.Constant.Roles.Categories.Page)]
        public async Task<Result<List<Category>>> GetAll()
        {
            var userInfo = GetUserInfo();

            var result = await _repository.GetAllAsync();

            if (!result.IsSuccess)
            {
                return result;
            }

            if (userInfo != null && userInfo.OrganizationId.HasValue)
            {
                var categories = result.Data?
                    .Where(c => c.OrganizationId == userInfo.OrganizationId.Value)
                    .ToList()
                    ?? new List<Category>();

                return new Result<List<Category>>(
                    result.IsSuccess,
                    result.Error,
                    categories
                );
            }

            return result;
        }

        [HttpGet]
        [ActionName("GetList")]
        [Authorize]
        public async Task<Result<List<Category>>> GetList()
        {
            var userInfo = GetUserInfo();

            var result = await _repository.GetListAsync();

            if (!result.IsSuccess)
            {
                return result;
            }

            if (userInfo != null && userInfo.OrganizationId.HasValue)
            {
                var categories = result.Data?
                    .Where(c => c.OrganizationId == userInfo.OrganizationId.Value)
                    .ToList()
                    ?? new List<Category>();

                return new Result<List<Category>>(
                    result.IsSuccess,
                    result.Error,
                    categories
                );
            }

            return result;
        }

        [HttpGet("{id}")]
        [ActionName("FindById")]
        [Authorize]
        public async Task<Result<Category>> FindById(Guid id)
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
                return Result.Failure<Category>(
                    new Shared.UnifiedResult.Error(
                        "غير مصرح بالوصول لهذا العنصر"
                    )
                );
            }

            return result;
        }

        [HttpPost]
        [ActionName("Add")]
        [Authorize(Roles = Shared.Constant.Roles.Categories.Add)]
        public async Task<Result<Category>> Add(
            [FromBody] Category category)
        {
            category = AddBaseInfo(category);

            var result = await _repository.AddAsync(category);

            return result;
        }

        [HttpPut("{id}")]
        [ActionName("Update")]
        [Authorize(Roles = Shared.Constant.Roles.Categories.Update)]
        public async Task<Result<Category>> Update(
            Guid id,
            [FromBody] Category category)
        {
            var userInfo = GetUserInfo();

            if (userInfo != null && userInfo.OrganizationId.HasValue)
            {
                category.OrganizationId = userInfo.OrganizationId;
            }

            var result = await _repository.UpdateAsync(id, category);

            return result;
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Shared.Constant.Roles.Categories.Delete)]
        public async Task<Result<Category>> Delete(Guid id)
        {
            var result = await _repository.DeleteAsync(id);

            return result;
        }
    }
}