using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AccountingERP.Data;
using AccountingERP.Models;

namespace AccountingERP.Services
{
    public interface IReportService
    {
        Task<AccountStatementReportResultDto> GetAccountStatementAsync(AccountStatementFilterViewModel filter);
        Task<TrialBalanceReportDto> GetTrialBalanceAsync(DateTime? fromDate, DateTime? toDate);
    }

    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AccountStatementReportResultDto> GetAccountStatementAsync(AccountStatementFilterViewModel filter)
        {
            var result = new AccountStatementReportResultDto
            {
                FromDate = filter.FromDate,
                ToDate = filter.ToDate
            };

            var account = await _context.Accounts
                .Include(a => a.ParentAccount)
                .FirstOrDefaultAsync(a => a.Id == filter.AccountId);

            if (account == null) return result;

            result.Account = new AccountDto
            {
                Id = account.Id,
                Code = account.Code,
                NameAr = account.NameAr,
                NameEn = account.NameEn,
                AccountType = account.AccountType,
                IsDebitNature = account.IsDebitNature,
                OpeningBalance = account.OpeningBalance
            };

            // Include account and any subaccounts if it's header or has subaccounts
            var accountIds = new List<Guid> { account.Id };
            var subIds = await _context.Accounts
                .Where(a => a.ParentAccountId == account.Id)
                .Select(a => a.Id)
                .ToListAsync();
            accountIds.AddRange(subIds);

            // Base query for details
            var detailsQuery = _context.DocumentDetails
                .Include(dd => dd.Document)
                .Include(dd => dd.CostCenter)
                .Where(dd => accountIds.Contains(dd.AccountId));

            if (filter.CostCenterId.HasValue && filter.CostCenterId.Value != Guid.Empty)
            {
                detailsQuery = detailsQuery.Where(dd => dd.CostCenterId == filter.CostCenterId.Value);
            }

            // 1. Calculate Opening Balance prior to FromDate
            decimal openingFromTrans = 0;
            if (filter.FromDate.HasValue)
            {
                var priorDetails = detailsQuery
                    .Where(dd => dd.Document!.DocumentDate < filter.FromDate.Value.Date);

                var priorDebit = await priorDetails.SumAsync(dd => (decimal)dd.Debit);
                var priorCredit = await priorDetails.SumAsync(dd => (decimal)dd.Credit);

                openingFromTrans = account.IsDebitNature ? (priorDebit - priorCredit) : (priorCredit - priorDebit);
            }

            result.OpeningBalance = account.OpeningBalance + openingFromTrans;

            // 2. Query transactions within period
            var periodQuery = detailsQuery;
            if (filter.FromDate.HasValue)
            {
                periodQuery = periodQuery.Where(dd => dd.Document!.DocumentDate >= filter.FromDate.Value.Date);
            }
            if (filter.ToDate.HasValue)
            {
                periodQuery = periodQuery.Where(dd => dd.Document!.DocumentDate <= filter.ToDate.Value.Date.AddDays(1).AddTicks(-1));
            }

            var detailsList = await periodQuery
                .OrderBy(dd => dd.Document!.DocumentDate)
                .ThenBy(dd => dd.Document!.DocumentNumber)
                .ToListAsync();

            decimal runningBalance = result.OpeningBalance;
            decimal totalDebit = 0;
            decimal totalCredit = 0;

            foreach (var detail in detailsList)
            {
                totalDebit += detail.Debit;
                totalCredit += detail.Credit;

                if (account.IsDebitNature)
                {
                    runningBalance += (detail.Debit - detail.Credit);
                }
                else
                {
                    runningBalance += (detail.Credit - detail.Debit);
                }

                result.Transactions.Add(new AccountStatementRowDto
                {
                    Date = detail.Document?.DocumentDate ?? DateTime.Today,
                    DocumentTypeTitle = detail.Document?.DocumentType switch
                    {
                        DocumentType.JournalVoucher => "قيد يومية",
                        DocumentType.PaymentVoucher => "سند صرف",
                        DocumentType.ReceiptVoucher => "سند قبض",
                        _ => "قيد"
                    },
                    DocumentNumber = detail.Document?.DocumentNumber ?? 0,
                    ReferenceNumber = detail.Document?.ReferenceNumber ?? string.Empty,
                    Notes = !string.IsNullOrWhiteSpace(detail.LineNotes) ? detail.LineNotes : (detail.Document?.Notes ?? string.Empty),
                    Debit = detail.Debit,
                    Credit = detail.Credit,
                    Balance = runningBalance,
                    CostCenterName = detail.CostCenter?.NameAr ?? string.Empty
                });
            }

            result.TotalDebit = totalDebit;
            result.TotalCredit = totalCredit;
            result.ClosingBalance = runningBalance;

            return result;
        }

        public async Task<TrialBalanceReportDto> GetTrialBalanceAsync(DateTime? fromDate, DateTime? toDate)
        {
            var from = fromDate ?? new DateTime(DateTime.Today.Year, 1, 1);
            var to = toDate ?? DateTime.Today;

            var accounts = await _context.Accounts
                .Where(a => a.AccountCategory == AccountCategory.Transactional)
                .OrderBy(a => a.Code)
                .ToListAsync();

            var result = new TrialBalanceReportDto
            {
                FromDate = from,
                ToDate = to
            };

            foreach (var acc in accounts)
            {
                var priorDebit = await _context.DocumentDetails
                    .Where(dd => dd.AccountId == acc.Id && dd.Document!.DocumentDate < from)
                    .SumAsync(dd => (decimal)dd.Debit);

                var priorCredit = await _context.DocumentDetails
                    .Where(dd => dd.AccountId == acc.Id && dd.Document!.DocumentDate < from)
                    .SumAsync(dd => (decimal)dd.Credit);

                var openDebit = acc.IsDebitNature ? acc.OpeningBalance + priorDebit : 0;
                var openCredit = !acc.IsDebitNature ? acc.OpeningBalance + priorCredit : 0;

                var periodDebit = await _context.DocumentDetails
                    .Where(dd => dd.AccountId == acc.Id && dd.Document!.DocumentDate >= from && dd.Document!.DocumentDate <= to)
                    .SumAsync(dd => (decimal)dd.Debit);

                var periodCredit = await _context.DocumentDetails
                    .Where(dd => dd.AccountId == acc.Id && dd.Document!.DocumentDate >= from && dd.Document!.DocumentDate <= to)
                    .SumAsync(dd => (decimal)dd.Credit);

                var endDebit = openDebit + periodDebit;
                var endCredit = openCredit + periodCredit;

                if (endDebit > endCredit)
                {
                    endDebit -= endCredit;
                    endCredit = 0;
                }
                else
                {
                    endCredit -= endDebit;
                    endDebit = 0;
                }

                result.Rows.Add(new TrialBalanceRowDto
                {
                    AccountCode = acc.Code,
                    AccountName = acc.NameAr,
                    OpeningDebit = openDebit,
                    OpeningCredit = openCredit,
                    PeriodDebit = periodDebit,
                    PeriodCredit = periodCredit,
                    EndingDebit = endDebit,
                    EndingCredit = endCredit
                });
            }

            result.TotalOpeningDebit = result.Rows.Sum(r => r.OpeningDebit);
            result.TotalOpeningCredit = result.Rows.Sum(r => r.OpeningCredit);
            result.TotalPeriodDebit = result.Rows.Sum(r => r.PeriodDebit);
            result.TotalPeriodCredit = result.Rows.Sum(r => r.PeriodCredit);
            result.TotalEndingDebit = result.Rows.Sum(r => r.EndingDebit);
            result.TotalEndingCredit = result.Rows.Sum(r => r.EndingCredit);

            return result;
        }
    }
}