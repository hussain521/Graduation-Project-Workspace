using Domain.DTOs;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DocumentsApiController : BaseController<Document>
    {
        DocumentRepository _repository;
        public DocumentsApiController(DocumentRepository repository)
        {
            this._repository = repository;
        }

        [HttpGet]
        [ActionName("GetAll")]
        [Authorize(Roles = Shared.Constant.Roles.Documents.Page)]
        public async Task<Result<List<Document>>> GetAll()
        {
            var Result = await this._repository.GetAllAsync();
            return Result;
        }

        [HttpGet]
        [ActionName("GetList")]
        [Authorize]
        public async Task<Result<List<Document>>> GetList()
        {
            var Result = await this._repository.GetListAsync();
            return Result;
        }

        [HttpGet("{id}")]
        [ActionName("FindById")]
        [Authorize]
        public async Task<Result<Document>> FindById(Guid id)
        {
            var Result = await this._repository.FindByIdAsync(id);
            return Result;
        }

        [HttpPost]
        [ActionName("Add")]
        [Authorize(Roles = Shared.Constant.Roles.Documents.Add)]
        public async Task<Result<Document>> Add([FromBody] Document document)
        {
            document = this.AddBaseInfo(document);
            var Result = await this._repository.AddAsync(document);
            return Result;
        }

        [HttpPut("{id}")]
        [ActionName("Update")]
        [Authorize(Roles = Shared.Constant.Roles.Documents.Update)]
        public async Task<Result<Document>> Update(Guid id, [FromBody] Document document)
        {
            var Result = await this._repository.UpdateAsync(id, document);
            return Result;
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Shared.Constant.Roles.Documents.Delete)]
        public async Task<Result<Document>> Delete(Guid id)
        {
            var Result = await this._repository.DeleteAsync(id);
            return Result;
        }

        [HttpPost]
        [Authorize]
        public Result<Document> RefreshSerialNum([FromBody]DocumentDTO documentDTO)
        {
            var Result = _repository.RefreshSerialNum(documentDTO);            
            return Result;
        }

        [HttpPost]
        [Authorize]
        public Result<List<AccountStatementViewModel>> GetAccountStatement([FromBody]Document dto)
        {
            var userInfo = this.GetUserInfo();
            if (userInfo != null && userInfo.OrganizationId.HasValue)
            {
                dto.OrganizationId = userInfo.OrganizationId;
            }
            var Result= _repository.GetAccountStatement(dto);
            return Result;
        }
    }
}
