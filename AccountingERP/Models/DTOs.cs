using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AccountingERP.Models
{
    // Account DTOs
    public class AccountDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string ParentAccountCode { get; set; } = string.Empty;
        public string ParentAccountName { get; set; } = string.Empty;
        public Guid? ParentAccountId { get; set; }
        public string AccountTypeTitle { get; set; } = string.Empty;
        public AccountType AccountType { get; set; }
        public string AccountCategoryTitle { get; set; } = string.Empty;
        public AccountCategory AccountCategory { get; set; }
        public int Level { get; set; }
        public decimal OpeningBalance { get; set; }
        public bool IsDebitNature { get; set; }
        public bool IsActive { get; set; }
        public decimal CurrentBalance { get; set; }
        public List<AccountDto> SubAccounts { get; set; } = new List<AccountDto>();
    }

    public class CreateUpdateAccountViewModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "رمز الحساب مطلوب")]
        [Display(Name = "رمز الحساب")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "اسم الحساب بالعربي مطلوب")]
        [Display(Name = "اسم الحساب (عربي)")]
        public string NameAr { get; set; } = string.Empty;

        [Display(Name = "اسم الحساب (إنجليزي)")]
        public string NameEn { get; set; } = string.Empty;

        [Display(Name = "الحساب الرئيسي")]
        public Guid? ParentAccountId { get; set; }

        [Required(ErrorMessage = "نوع الحساب مطلوب")]
        [Display(Name = "نوع الحساب")]
        public AccountType AccountType { get; set; }

        [Required(ErrorMessage = "فئة الحساب مطلوبة")]
        [Display(Name = "فئة الحساب")]
        public AccountCategory AccountCategory { get; set; }

        [Display(Name = "الرصيد الافتتاحي")]
        public decimal OpeningBalance { get; set; } = 0;

        [Display(Name = "طبيعة الحساب دائن/مدين")]
        public bool IsDebitNature { get; set; } = true;

        [Display(Name = "الحالة")]
        public bool IsActive { get; set; } = true;
    }

    // Document / Voucher DTOs & ViewModels
    public class DocumentDetailsViewModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "يجب اختيار الحساب")]
        public Guid AccountId { get; set; }
        public string AccountCode { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;

        public Guid? CostCenterId { get; set; }
        public string CostCenterName { get; set; } = string.Empty;

        [Range(0, 999999999, ErrorMessage = "المبلغ المدين يجب أن يكون موجبًا")]
        public decimal Debit { get; set; } = 0;

        [Range(0, 999999999, ErrorMessage = "المبلغ الدائن يجب أن يكون موجبًا")]
        public decimal Credit { get; set; } = 0;

        public string LineNotes { get; set; } = string.Empty;
    }

    public class CreateUpdateDocumentViewModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "نوع المستند مطلوب")]
        public DocumentType DocumentType { get; set; }

        public long DocumentNumber { get; set; }

        [Required(ErrorMessage = "تاريخ المستند مطلوب")]
        [DataType(DataType.Date)]
        public DateTime DocumentDate { get; set; } = DateTime.Today;

        [Display(Name = "رقم المرجع")]
        public string ReferenceNumber { get; set; } = string.Empty;

        [Display(Name = "البيان / الشرح")]
        public string Notes { get; set; } = string.Empty;

        public DocumentStatus Status { get; set; } = DocumentStatus.Draft;

        public List<DocumentDetailsViewModel> Details { get; set; } = new List<DocumentDetailsViewModel>();
    }

    public class DocumentDto
    {
        public Guid Id { get; set; }
        public string DocumentTypeTitle { get; set; } = string.Empty;
        public DocumentType DocumentType { get; set; }
        public long DocumentNumber { get; set; }
        public string DocumentDateFormatted { get; set; } = string.Empty;
        public DateTime DocumentDate { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string StatusTitle { get; set; } = string.Empty;
        public DocumentStatus Status { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public List<DocumentDetailsDto> Details { get; set; } = new List<DocumentDetailsDto>();
    }

    public class DocumentDetailsDto
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string AccountCode { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string CostCenterName { get; set; } = string.Empty;
        public Guid? CostCenterId { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string LineNotes { get; set; } = string.Empty;
    }

    // Reports Models (Accurate Account Statement)
    public class AccountStatementFilterViewModel
    {
        [Required(ErrorMessage = "يجب اختيار الحساب")]
        public Guid AccountId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        public Guid? CostCenterId { get; set; }
    }

    public class AccountStatementRowDto
    {
        public DateTime Date { get; set; }
        public string DateFormatted => Date.ToString("yyyy-MM-dd");
        public string DocumentTypeTitle { get; set; } = string.Empty;
        public long DocumentNumber { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; } // Running Balance
        public string CostCenterName { get; set; } = string.Empty;
    }

    public class AccountStatementReportResultDto
    {
        public AccountDto Account { get; set; } = new AccountDto();
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal NetMovement => TotalDebit - TotalCredit;
        public decimal ClosingBalance { get; set; }
        public List<AccountStatementRowDto> Transactions { get; set; } = new List<AccountStatementRowDto>();
    }

    // Trial Balance DTO
    public class TrialBalanceRowDto
    {
        public string AccountCode { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public decimal OpeningDebit { get; set; }
        public decimal OpeningCredit { get; set; }
        public decimal PeriodDebit { get; set; }
        public decimal PeriodCredit { get; set; }
        public decimal EndingDebit { get; set; }
        public decimal EndingCredit { get; set; }
    }

    public class TrialBalanceReportDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<TrialBalanceRowDto> Rows { get; set; } = new List<TrialBalanceRowDto>();
        public decimal TotalOpeningDebit { get; set; }
        public decimal TotalOpeningCredit { get; set; }
        public decimal TotalPeriodDebit { get; set; }
        public decimal TotalPeriodCredit { get; set; }
        public decimal TotalEndingDebit { get; set; }
        public decimal TotalEndingCredit { get; set; }
    }

    // User & Permissions ViewModels
    public class UserViewModel
    {
        public Guid? Id { get; set; }
        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        public string Username { get; set; } = string.Empty;
        [Display(Name = "كلمة المرور")]
        public string? Password { get; set; }
        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public Guid? RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}