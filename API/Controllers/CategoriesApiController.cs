namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoriesApiController : BaseController<Category>
    {
        private readonly CategoryRepository _repository;

        public CategoriesApiController(CategoryRepository repository)
        {
            this._repository = repository;
        }

        [HttpGet]
        [ActionName("GetAll")]
        [Authorize(Roles = Shared.Constant.Roles.Categories.Page)]
        public async Task<Result<List<Category>>> GetAll()
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
        public async Task<Result<List<Category>>> GetList()
        {
            var userInfo = this.GetUserInfo();
            var Result = await this._repository.GetListAsync();
            if (Result.IsSuccess && userInfo != null && userInfo.OrganizationId.HasValue)
            {
                Result.Data = Result.Data.Where(c => c.OrganizationId == userInfo.OrganizationId).ToList();
            }
            return Result;
        }

        [HttpGet("{id}")]
        [ActionName("FindById")]
        [Authorize]
        public async Task<Result<Category>> FindById(Guid id)
        {
            var userInfo = this.GetUserInfo();
            var Result = await this._repository.FindByIdAsync(id);
            if (Result.IsSuccess && Result.Data != null && userInfo != null && userInfo.OrganizationId.HasValue)
            {
                if (Result.Data.OrganizationId != userInfo.OrganizationId)
                {
                    return Result.Failure<Category>(new Shared.Constant.Roles.Error("غير مصرح بالوصول لهذا العنصر"));
                }
            }
            return Result;
        }

        [HttpPost]
        [ActionName("Add")]
        [Authorize(Roles = Shared.Constant.Roles.Categories.Add)]
        public async Task<Result<Category>> Add([FromBody] Category category)
        {
            category = this.AddBaseInfo(category);
            var Result = await this._repository.AddAsync(category);
            return Result;
        }

        [HttpPut("{id}")]
        [ActionName("Update")]
        [Authorize(Roles = Shared.Constant.Roles.Categories.Update)]
        public async Task<Result<Category>> Update(Guid id, [FromBody] Category category)
        {
            var userInfo = this.GetUserInfo();
            if (userInfo != null && userInfo.OrganizationId.HasValue)
            {
                category.OrganizationId = userInfo.OrganizationId;
            }
            var Result = await this._repository.UpdateAsync(id, category);
            return Result;
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Shared.Constant.Roles.Categories.Delete)]
        public async Task<Result<Category>> Delete(Guid id)
        {
            var Result = await this._repository.DeleteAsync(id);
            return Result;
        }
    }
}