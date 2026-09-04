namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ItemsApiController : BaseController<Item>
    {
        ItemRepository _repository; 
        public ItemsApiController(ItemRepository repository)
        {
            this._repository = repository;
        }

        // GET: api/<ItemsApiController>
        [HttpGet]
        [ActionName("GetAll")]
        [Authorize(Roles = Shared.Constant.Roles.Items.Page)]
        public async Task<Result<List<Item>>> GetAll()
        {
            var userInfo = this.GetUserInfo();
            var Result = await this._repository.GetAllAsync();
            if (Result.IsSuccess && userInfo != null && userInfo.OrganizationId.HasValue)
            {
                Result.Data = Result.Data.Where(i => i.OrganizationId == userInfo.OrganizationId).ToList();
            }
            return Result;
        }

        [HttpGet]
        [ActionName("GetList")]
        [Authorize]
        public async Task<Result<List<Item>>> GetList()
        {
            var userInfo = this.GetUserInfo();
            var Result = await this._repository.GetListAsync();
            if (Result.IsSuccess && userInfo != null && userInfo.OrganizationId.HasValue)
            {
                Result.Data = Result.Data.Where(i => i.OrganizationId == userInfo.OrganizationId).ToList();
            }
            return Result;
        }

        // GET api/<ItemsApiController>/5
        [HttpGet("{id}")]
        [ActionName("FindById")]
        [Authorize]
        public async Task<Result<Item>> FindById(Guid id)
        {
            var userInfo = this.GetUserInfo();
            var Result = await this._repository.FindByIdAsync(id);
            if (Result.IsSuccess && Result.Data != null && userInfo != null && userInfo.OrganizationId.HasValue)
            {
                if (Result.Data.OrganizationId != userInfo.OrganizationId)
                {
                    return Result.Failure<Item>(new Shared.Constant.Roles.Error("غير مصرح بالوصول لهذا الصنف"));
                }
            }
            return Result;
        }

        // POST api/<ItemsApiController>
        [HttpPost]
        [ActionName("Add")]
        [Authorize(Roles = Shared.Constant.Roles.Items.Add)]
        public async Task<Result<Item>> Add([FromBody] Item item)
        {
            item = this.AddBaseInfo(item);
            var Result = await this._repository.AddAsync(item);
            return Result;
        }

        // PUT api/<ItemsApiController>/5
        [HttpPut("{id}")]
        [ActionName("Update")]
        [Authorize(Roles = Shared.Constant.Roles.Items.Update)]
        public async Task<Result<Item>> Update(Guid id, [FromBody] Item item)
        {
            var userInfo = this.GetUserInfo();
            if (userInfo != null && userInfo.OrganizationId.HasValue)
            {
                item.OrganizationId = userInfo.OrganizationId;
            }
            var Result = await this._repository.UpdateAsync(id, item);
            return Result;
        }

        // DELETE api/<ItemsApiController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = Shared.Constant.Roles.Items.Delete)]
        public async Task<Result<Item>> Delete(Guid id)
        {
            var Result = await this._repository.DeleteAsync(id);
            return Result; 
        }
    }
}
