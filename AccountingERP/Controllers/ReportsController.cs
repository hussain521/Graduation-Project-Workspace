using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AccountingERP.Models;
using AccountingERP.Services;

namespace AccountingERP.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IAccountService _accountService;

        public ReportsController(IReportService reportService, IAccountService accountService)
        {
            _reportService = reportService;
            _accountService = accountService;
        }

        public async Task<IActionResult> Statement(AccountStatementFilterViewModel filter)
        {
            var accounts = await _accountService.GetAllAccountsAsync();
            ViewBag.Accounts = accounts;

            if (filter.AccountId == Guid.Empty && accounts.Count > 0)
            {
                var defaultAcc = accounts.Find(a => a.AccountCategory == AccountCategory.Transactional) ?? accounts[0];
                filter.AccountId = defaultAcc.Id;
            }

            AccountStatementReportResultDto statement = new AccountStatementReportResultDto();
            if (filter.AccountId != Guid.Empty)
            {
                statement = await _reportService.GetAccountStatementAsync(filter);
            }

            ViewBag.Filter = filter;
            return View(statement);
        }

        public async Task<IActionResult> TrialBalance(DateTime? fromDate, DateTime? toDate)
        {
            var trialBalance = await _reportService.GetTrialBalanceAsync(fromDate, toDate);
            return View(trialBalance);
        }
    }
}