using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AccountingERP.Services;

namespace AccountingERP.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IDocumentService _documentService;

        public HomeController(IAccountService accountService, IDocumentService documentService)
        {
            _accountService = accountService;
            _documentService = documentService;
        }

        public async Task<IActionResult> Index()
        {
            var accounts = await _accountService.GetAllAccountsAsync();
            var documents = await _documentService.GetDocumentsAsync();

            ViewBag.TotalAccounts = accounts.Count;
            ViewBag.TotalDocuments = documents.Count;
            ViewBag.PostedDocuments = documents.FindAll(d => d.Status == Models.DocumentStatus.Posted).Count;

            // Cash and Bank Balances
            var cashAndBank = accounts.FindAll(a => a.Code.StartsWith("1101") || a.Code.StartsWith("1102"));
            ViewBag.CashAndBankCount = cashAndBank.Count;

            ViewBag.RecentDocuments = documents.GetRange(0, System.Math.Min(5, documents.Count));

            return View();
        }
    }
}