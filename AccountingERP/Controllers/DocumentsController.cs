using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AccountingERP.Models;
using AccountingERP.Services;

namespace AccountingERP.Controllers
{
    [Authorize]
    public class DocumentsController : Controller
    {
        private readonly IDocumentService _documentService;
        private readonly IAccountService _accountService;

        public DocumentsController(IDocumentService documentService, IAccountService accountService)
        {
            _documentService = documentService;
            _accountService = accountService;
        }

        public async Task<IActionResult> Index(DocumentType? type = null, DocumentStatus? status = null, string? search = null)
        {
            var documents = await _documentService.GetDocumentsAsync(type, status, search: search);
            ViewBag.CurrentType = type;
            ViewBag.CurrentStatus = status;
            ViewBag.Search = search;
            return View(documents);
        }

        [HttpGet]
        public async Task<IActionResult> Create(DocumentType type = DocumentType.JournalVoucher)
        {
            var nextNum = await _documentService.GetNextDocumentNumberAsync(type);
            var accounts = await _accountService.GetTransactionalAccountsAsync();

            ViewBag.Accounts = accounts;
            var model = new CreateUpdateDocumentViewModel
            {
                DocumentType = type,
                DocumentNumber = nextNum,
                DocumentDate = DateTime.Today
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] CreateUpdateDocumentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "بيانات المستند غير مكتملة أو تحتوي أخطاء" });
            }

            var result = await _documentService.CreateDocumentAsync(model);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var doc = await _documentService.GetDocumentByIdAsync(id);
            if (doc == null) return NotFound();

            var accounts = await _accountService.GetTransactionalAccountsAsync();
            ViewBag.Accounts = accounts;

            var model = new CreateUpdateDocumentViewModel
            {
                Id = doc.Id,
                DocumentType = doc.DocumentType,
                DocumentNumber = doc.DocumentNumber,
                DocumentDate = doc.DocumentDate,
                ReferenceNumber = doc.ReferenceNumber,
                Notes = doc.Notes,
                Status = doc.Status,
                Details = doc.Details.ConvertAll(d => new DocumentDetailsViewModel
                {
                    Id = d.Id,
                    AccountId = d.AccountId,
                    AccountCode = d.AccountCode,
                    AccountName = d.AccountName,
                    CostCenterId = d.CostCenterId,
                    CostCenterName = d.CostCenterName,
                    Debit = d.Debit,
                    Credit = d.Credit,
                    LineNotes = d.LineNotes
                })
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] CreateUpdateDocumentViewModel model)
        {
            var result = await _documentService.UpdateDocumentAsync(model);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _documentService.DeleteDocumentAsync(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post(Guid id)
        {
            var result = await _documentService.PostDocumentAsync(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unpost(Guid id)
        {
            var result = await _documentService.UnpostDocumentAsync(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Json(result);
        }
    }
}