using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AccountingERP.Models;
using AccountingERP.Services;

namespace AccountingERP.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsApiController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsApiController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<ActionResult<List<AccountDto>>> GetAll()
        {
            var accounts = await _accountService.GetAllAccountsAsync();
            return Ok(accounts);
        }

        [HttpGet("tree")]
        public async Task<ActionResult<List<AccountDto>>> GetTree()
        {
            var tree = await _accountService.GetAccountTreeAsync();
            return Ok(tree);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AccountDto>> GetById(Guid id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account == null) return NotFound(new { message = "الحساب غير موجود" });
            return Ok(account);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<AccountDto>>> Create([FromBody] CreateUpdateAccountViewModel model)
        {
            var result = await _accountService.CreateAccountAsync(model);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<AccountDto>>> Update(Guid id, [FromBody] CreateUpdateAccountViewModel model)
        {
            model.Id = id;
            var result = await _accountService.UpdateAccountAsync(model);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var result = await _accountService.DeleteAccountAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsApiController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentsApiController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet]
        public async Task<ActionResult<List<DocumentDto>>> GetDocuments([FromQuery] DocumentType? type, [FromQuery] DocumentStatus? status)
        {
            var docs = await _documentService.GetDocumentsAsync(type, status);
            return Ok(docs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentDto>> GetById(Guid id)
        {
            var doc = await _documentService.GetDocumentByIdAsync(id);
            if (doc == null) return NotFound(new { message = "المستند غير موجود" });
            return Ok(doc);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<DocumentDto>>> Create([FromBody] CreateUpdateDocumentViewModel model)
        {
            var res = await _documentService.CreateDocumentAsync(model);
            if (!res.Success) return BadRequest(res);
            return Ok(res);
        }

        [HttpPost("{id}/post")]
        public async Task<ActionResult<ApiResponse<bool>>> Post(Guid id)
        {
            var res = await _documentService.PostDocumentAsync(id);
            if (!res.Success) return BadRequest(res);
            return Ok(res);
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ReportsApiController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsApiController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpPost("statement")]
        public async Task<ActionResult<AccountStatementReportResultDto>> GetStatement([FromBody] AccountStatementFilterViewModel filter)
        {
            var result = await _reportService.GetAccountStatementAsync(filter);
            return Ok(result);
        }

        [HttpGet("trial-balance")]
        public async Task<ActionResult<TrialBalanceReportDto>> GetTrialBalance([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var result = await _reportService.GetTrialBalanceAsync(fromDate, toDate);
            return Ok(result);
        }
    }
}