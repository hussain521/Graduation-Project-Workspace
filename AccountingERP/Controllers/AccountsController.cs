using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AccountingERP.Models;
using AccountingERP.Services;

namespace AccountingERP.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<IActionResult> Index()
        {
            var tree = await _accountService.GetAccountTreeAsync();
            var allAccounts = await _accountService.GetAllAccountsAsync();
            ViewBag.AllAccounts = allAccounts;
            return View(tree);
        }

        [HttpGet]
        public async Task<IActionResult> GetTreeJson()
        {
            var tree = await _accountService.GetAccountTreeAsync();
            return Json(tree);
        }

        [HttpGet]
        public async Task<IActionResult> GetAccount(Guid id)
        {
            var acc = await _accountService.GetAccountByIdAsync(id);
            if (acc == null) return NotFound(new { message = "الحساب غير موجود" });
            return Json(acc);
        }

        [HttpGet]
        public async Task<IActionResult> GetNextCode(Guid? parentId, AccountType type)
        {
            var code = await _accountService.GenerateNextAccountCodeAsync(parentId, type);
            return Json(new { code });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] CreateUpdateAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "البيانات المدخلة غير صحيحة", errors = ModelState });
            }

            var result = await _accountService.CreateAccountAsync(model);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] CreateUpdateAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "البيانات المدخلة غير صحيحة" });
            }

            var result = await _accountService.UpdateAccountAsync(model);
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
            var result = await _accountService.DeleteAccountAsync(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Json(result);
        }
    }
}