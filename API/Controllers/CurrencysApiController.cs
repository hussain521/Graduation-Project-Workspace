using API.Controllers.Base;
using Domain.Entities;
using Infrastructure.Repositories.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Constant;
using Shared.UnifiedResult;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CurrencysApiController : BaseApiController<Currency>
    {
        GenericRepository<Currency> _repository;
        public CurrencysApiController(GenericRepository<Currency> repository)
        {
            this._repository = repository;
        }
        // GET: api/<CurrencysApiController>
        [HttpGet]
        [ActionName("GetAll")]
        [Authorize(Roles = Shared.Constant.Roles.Currencies.Page)]
        public async Task<Result<List<Currency>>> GetAll()
        {
            var Result = await this._repository.GetAllAsync();
            return Result;
        }

        // GET api/<CurrencysApiController>/5
        [HttpGet("{id}")]
        [ActionName("FindById")]
        [Authorize]
        public async Task<Result<Currency>> FindById(Guid id)
        {
            var Result = await this._repository.FindByIdAsync(id);
            return Result;
        }

        // POST api/<CurrencysApiController>
        [HttpPost]
        [ActionName("Add")]
        [Authorize(Roles = Shared.Constant.Roles.Currencies.Add)]
        public async Task<Result<Currency>> Add([FromBody] Currency Currency)
        {
            Currency = this.AddBaseInfo(Currency);
            var Result = await this._repository.AddAsync(Currency);
            return Result;
        }

        // PUT api/<CurrencysApiController>/5
        [HttpPut("{id}")]
        [ActionName("Update")]
        [Authorize(Roles = Shared.Constant.Roles.Currencies.Update)]
        public async Task<Result<Currency>> Update(Guid id, [FromBody] Currency Currency)
        {
            var Result = await this._repository.UpdateAsync(id, Currency);
            return Result;
        }

        // DELETE api/<CurrencysApiController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = Shared.Constant.Roles.Currencies.Delete)]
        public async Task<Result<Currency>> Delete(Guid id)
        {
            var Result = await this._repository.DeleteAsync(id);
            return Result;
        }
    }
}
