using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AccountingERP.Models
{
    // Generic Service Result
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new List<string>();

        public static ServiceResult Ok(string message = "تمت العملية بنجاح") =>
            new ServiceResult { Success = true, Message = message };

        public static ServiceResult Fail(string error) =>
            new ServiceResult { Success = false, Message = error, Errors = new List<string> { error } };

        public static ServiceResult Fail(List<string> errors, string message = "حدث خطأ أثناء تنفيذ العملية") =>
            new ServiceResult { Success = false, Message = message, Errors = errors };
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; set; }

        public static ServiceResult<T> Ok(T data, string message = "تمت العملية بنجاح") =>
            new ServiceResult<T> { Success = true, Message = message, Data = data };

        public new static ServiceResult<T> Fail(string error) =>
            new ServiceResult<T> { Success = false, Message = error, Errors = new List<string> { error } };
    }

    // Login & Auth ViewModels
    public class LoginViewModel
    {
        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        [Display(Name = "اسم المستخدم")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }

    // Dashboard ViewModels
    public class DashboardViewModel
    {
        public decimal TotalAssets { get; set; }
        public decimal TotalLiabilities { get; set; }
        public decimal TotalRevenues { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit => TotalRevenues - TotalExpenses;
        public decimal CashAndBankBalance { get; set; }

        public int TotalAccounts { get; set; }
        public int TotalJournalEntries { get; set; }
        public int DraftDocumentsCount { get; set; }
        public int PostedDocumentsCount { get; set; }

        public string OrganizationName { get; set; } = string.Empty;
        public string CurrencySymbol { get; set; } = "ر.س";

        public List<RecentDocumentDto> RecentDocuments { get; set; } = new List<RecentDocumentDto>();
        public List<AccountBalanceSummaryDto> KeyAccounts { get; set; } = new List<AccountBalanceSummaryDto>();
    }

    public class RecentDocumentDto
    {
        public Guid DocumentId { get; set; }
        public string DocumentCode { get; set; } = string.Empty;
        public string DocumentTypeName { get; set; } = string.Empty;
        public DateTime DocumentDate { get; set; }
        public decimal TotalAmount { get; set; }
        public DocumentStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class AccountBalanceSummaryDto
    {
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string Nature { get; set; } = string.Empty;
    }

    // Account Statement Report Models (كشف الحساب)
    public class StatementReportFilter
    {
        [Required(ErrorMessage = "يرجى تحديد الحساب")]
        public Guid AccountId { get; set; }

        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; } = new DateTime(DateTime.Today.Year, 1, 1);

        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; } = DateTime.Today;

        public Guid? CostCenterId { get; set; }
        public Guid? BranchId { get; set; }
        public bool IncludeDraft { get; set; } = false;
    }

    public class StatementReportViewModel
    {
        public StatementReportFilter Filter { get; set; } = new StatementReportFilter();

        // Account Details (No Raw GUIDs displayed)
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountTypeName { get; set; } = string.Empty;
        public string AccountNatureName { get; set; } = string.Empty;
        public string CurrencySymbol { get; set; } = "ر.س";

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        // Opening Balance
        public decimal OpeningBalance { get; set; }
        public string OpeningBalanceNature { get; set; } = string.Empty;

        // Transactions
        public List<StatementTransactionRow> Rows { get; set; } = new List<StatementTransactionRow>();

        // Period Totals
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal NetMovement => TotalDebit - TotalCredit;

        // Closing Balance
        public decimal ClosingBalance { get; set; }
        public string ClosingBalanceNature { get; set; } = string.Empty;
    }

    public class StatementTransactionRow
    {
        public DateTime Date { get; set; }
        public string DocumentCode { get; set; } = string.Empty;
        public string DocumentTypeName { get; set; } = string.Empty;
        public string? ReferenceNumber { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public string? CostCenterName { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal RunningBalance { get; set; }
        public string RunningBalanceNature { get; set; } = string.Empty;
    }

    // Trial Balance (ميزان المراجعة)
    public class TrialBalanceReportViewModel
    {
        public DateTime AsOfDate { get; set; } = DateTime.Today;
        public int Level { get; set; } = 4;
        public string CurrencySymbol { get; set; } = "ر.س";

        public List<TrialBalanceItemDto> Items { get; set; } = new List<TrialBalanceItemDto>();

        public decimal TotalOpeningDebit { get; set; }
        public decimal TotalOpeningCredit { get; set; }
        public decimal TotalMovementDebit { get; set; }
        public decimal TotalMovementCredit { get; set; }
        public decimal TotalEndingDebit { get; set; }
        public decimal TotalEndingCredit { get; set; }
    }

    public class TrialBalanceItemDto
    {
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public int Level { get; set; }
        public bool IsParent { get; set; }

        public decimal OpeningDebit { get; set; }
        public decimal OpeningCredit { get; set; }
        public decimal MovementDebit { get; set; }
        public decimal MovementCredit { get; set; }
        public decimal EndingDebit { get; set; }
        public decimal EndingCredit { get; set; }
    }

    // Document / Journal Entry View Models
    public class DocumentFormViewModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "نوع المستند مطلوب")]
        public DocumentType DocumentType { get; set; } = DocumentType.JournalEntry;

        [Required(ErrorMessage = "تاريخ المستند مطلوب")]
        [DataType(DataType.Date)]
        public DateTime DocumentDate { get; set; } = DateTime.Today;

        public Guid BranchId { get; set; }
        public Guid FiscalYearId { get; set; }

        [MaxLength(100)]
        public string? ReferenceNumber { get; set; }

        [Required(ErrorMessage = "البيان / الشرح العام مطلوب")]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public DocumentStatus Status { get; set; } = DocumentStatus.Draft;
        public string? DocumentCode { get; set; }

        public List<DocumentDetailItemViewModel> Details { get; set; } = new List<DocumentDetailItemViewModel>();

        // Display totals
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal Difference => Math.Abs(TotalDebit - TotalCredit);
        public bool IsBalanced => Math.Abs(TotalDebit - TotalCredit) < 0.001m && TotalDebit > 0;
    }

    public class DocumentDetailItemViewModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "الحساب مطلوب")]
        public Guid AccountId { get; set; }

        public string? AccountNumber { get; set; }
        public string? AccountName { get; set; }

        public Guid? CostCenterId { get; set; }
        public string? CostCenterName { get; set; }

        public decimal Debit { get; set; }
        public decimal Credit { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }
    }

    // User & Role Management ViewModels
    public class UserFormViewModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public Guid? RoleId { get; set; }
        public string? RoleName { get; set; }

        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class RoleFormViewModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "اسم الدور مطلوب")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public List<string> SelectedPermissions { get; set; } = new List<string>();
    }
}