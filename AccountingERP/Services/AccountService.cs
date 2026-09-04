using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountingERP.Data;
using AccountingERP.Models;
using Microsoft.EntityFrameworkCore;

namespace AccountingERP.Services
{
    public interface IAccountService
    {
        Task<List<Account>> GetAllAccountsAsync(bool includeInactive = false);
        Task<List<Account>> GetLeafAccountsAsync(); // Accounts that can receive transactions
        Task<Account?> GetAccountByIdAsync(Guid id);
        Task<Account?> GetAccountByNumberAsync(string accountNumber);
        Task<ServiceResult<Account>> CreateAccountAsync(Account account);
        Task<ServiceResult<Account>> UpdateAccountAsync(Account account);
        Task<ServiceResult> DeleteAccountAsync(Guid id);
        Task<string> SuggestNextAccountNumberAsync(Guid? parentAccountId, AccountType type);
        Task<List<AccountTreeNodeDto>> GetAccountTreeAsync();
    }

    public class AccountTreeNodeDto
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string? NameEn { get; set; }
        public AccountType AccountType { get; set; }
        public string AccountTypeName { get; set; } = string.Empty;
        public AccountNature AccountNature { get; set; }
        public string AccountNatureName { get; set; } = string.Empty;
        public int Level { get; set; }
        public bool IsParent { get; set; }
        public bool IsActive { get; set; }
        public decimal CurrentBalance { get; set; }
        public Guid? ParentAccountId { get; set; }
        public List<AccountTreeNodeDto> Children { get; set; } = new List<AccountTreeNodeDto>();
    }

    public class AccountService : IAccountService
    {
        private readonly AppDbContext _context;
        private readonly ITenantService _tenantService;

        public AccountService(AppDbContext context, ITenantService tenantService)
        {
            _context = context;
            _tenantService = tenantService;
        }

        public async Task<List<Account>> GetAllAccountsAsync(bool includeInactive = false)
        {
            var query = _context.Accounts
                .Include(a => a.ParentAccount)
                .AsQueryable();

            if (!includeInactive)
                query = query.Where(a => a.IsActive);

            return await query.OrderBy(a => a.AccountNumber).ToListAsync();
        }

        public async Task<List<Account>> GetLeafAccountsAsync()
        {
            return await _context.Accounts
                .Where(a => a.IsActive && !a.IsParent)
                .OrderBy(a => a.AccountNumber)
                .ToListAsync();
        }

        public async Task<Account?> GetAccountByIdAsync(Guid id)
        {
            return await _context.Accounts
                .Include(a => a.ParentAccount)
                .Include(a => a.SubAccounts)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Account?> GetAccountByNumberAsync(string accountNumber)
        {
            return await _context.Accounts
                .Include(a => a.ParentAccount)
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
        }

        public async Task<ServiceResult<Account>> CreateAccountAsync(Account account)
        {
            // Validations
            if (string.IsNullOrWhiteSpace(account.AccountNumber))
                return ServiceResult<Account>.Fail("رقم الحساب مطلوب");

            if (string.IsNullOrWhiteSpace(account.NameAr))
                return ServiceResult<Account>.Fail("اسم الحساب باللغة العربية مطلوب");

            var orgId = _tenantService.OrganizationId;
            if (orgId == Guid.Empty)
                return ServiceResult<Account>.Fail("لم يتم تحديد المؤسسة");

            account.OrganizationId = orgId;

            // Check unique number
            var exists = await _context.Accounts
                .AnyAsync(a => a.OrganizationId == orgId && a.AccountNumber == account.AccountNumber);
            if (exists)
                return ServiceResult<Account>.Fail($"رقم الحساب '{account.AccountNumber}' موجود مسبقاً");

            // Setup Parent & Level
            if (account.ParentAccountId.HasValue && account.ParentAccountId.Value != Guid.Empty)
            {
                var parent = await _context.Accounts.FindAsync(account.ParentAccountId.Value);
                if (parent == null)
                    return ServiceResult<Account>.Fail("الحساب الرئيسي المحدد غير موجود");

                account.Level = parent.Level + 1;
                account.AccountType = parent.AccountType;
                account.AccountNature = parent.AccountNature;

                if (!parent.IsParent)
                {
                    // If parent had transactions, prevent converting to parent or warn
                    var hasTx = await _context.DocumentDetails.AnyAsync(d => d.AccountId == parent.Id);
                    if (hasTx)
                    {
                        return ServiceResult<Account>.Fail("لا يمكن تفريع هذا الحساب لأن لديه قيود وحركات سابقة");
                    }
                    parent.IsParent = true;
                    _context.Accounts.Update(parent);
                }
            }
            else
            {
                account.Level = 1;
                account.ParentAccountId = null;
            }

            account.CreatedAt = DateTime.UtcNow;
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return ServiceResult<Account>.Ok(account, "تمت إضافة الحساب بنجاح");
        }

        public async Task<ServiceResult<Account>> UpdateAccountAsync(Account account)
        {
            var existing = await _context.Accounts.FindAsync(account.Id);
            if (existing == null)
                return ServiceResult<Account>.Fail("الحساب غير موجود");

            if (string.IsNullOrWhiteSpace(account.NameAr))
                return ServiceResult<Account>.Fail("اسم الحساب مطلوب");

            // Check number uniqueness if changed
            if (existing.AccountNumber != account.AccountNumber)
            {
                var numExists = await _context.Accounts
                    .AnyAsync(a => a.Id != account.Id && a.OrganizationId == existing.OrganizationId && a.AccountNumber == account.AccountNumber);
                if (numExists)
                    return ServiceResult<Account>.Fail($"رقم الحساب '{account.AccountNumber}' مستخدم في حساب آخر");

                existing.AccountNumber = account.AccountNumber;
            }

            existing.NameAr = account.NameAr;
            existing.NameEn = account.NameEn;
            existing.IsActive = account.IsActive;
            existing.Notes = account.Notes;

            if (!existing.IsParent)
            {
                existing.AccountType = account.AccountType;
                existing.AccountNature = account.AccountNature;
                existing.OpeningBalance = account.OpeningBalance;
            }

            _context.Accounts.Update(existing);
            await _context.SaveChangesAsync();

            return ServiceResult<Account>.Ok(existing, "تم تعديل بيانات الحساب بنجاح");
        }

        public async Task<ServiceResult> DeleteAccountAsync(Guid id)
        {
            var account = await _context.Accounts
                .Include(a => a.SubAccounts)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (account == null)
                return ServiceResult.Fail("الحساب غير موجود");

            if (account.SubAccounts.Any(s => s.IsActive))
                return ServiceResult.Fail("لا يمكن حذف الحساب لوجود حسابات فرعية تابعة له");

            var hasDetails = await _context.DocumentDetails.AnyAsync(d => d.AccountId == id);
            if (hasDetails)
                return ServiceResult.Fail("لا يمكن حذف الحساب لوجود حركات وقيود محاسبية مسجلة عليه. يمكنك تعطيل الحساب بدلاً من حذفه.");

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return ServiceResult.Ok("تم حذف الحساب بنجاح");
        }

        public async Task<string> SuggestNextAccountNumberAsync(Guid? parentAccountId, AccountType type)
        {
            var orgId = _tenantService.OrganizationId;
            if (parentAccountId<execute_command>
<command>dotnet new mvc -n AccountingERP -f net8.0</command>
</execute_command><execute_command>
<command>cd AccountingERP && dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 8.0.11 && dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.11 && dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.11 && dotnet add package Swashbuckle.AspNetCore --version 6.6.2</command>
</execute_command><execute_command>
<command>cd AccountingERP && dotnet build</command>
</execute_command><execute_command>
<command>dotnet new mvc -n AccountingERP --no-https</command>
</execute_command><execute_command>
<command>dotnet add AccountingERP/AccountingERP.csproj package Microsoft.EntityFrameworkCore.Sqlite --version 8.0.8 && dotnet add AccountingERP/AccountingERP.csproj package Microsoft.EntityFrameworkCore.Tools --version 8.0.8 && dotnet add AccountingERP/AccountingERP.csproj package BCrypt.Net-Next --version 4.0.3 && dotnet add AccountingERP/AccountingERP.csproj package Swashbuckle.AspNetCore --version 6.6.2 && dotnet add AccountingERP/AccountingERP.csproj package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.8</command>
</execute_command><write_file>
<path>AccountingERP/Models/Entities.cs</path>
<line_count>265</line_count>
<content>using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountingERP.Models
{
    public class Organization
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required, MaxLength(150)]
        public string NameAr { get; set; } = string.Empty;
        [MaxLength(150)]
        public string NameEn { get; set; } = string.Empty;
        [MaxLength(50)]
        public string TaxNumber { get; set; } = string.Empty;
        [MaxLength(20)]
        public string CommercialRegistration { get; set; } = string.Empty;
        [MaxLength(200)]
        public string Address { get; set; } = string.Empty;
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Branch> Branches { get; set; } = new List<Branch>();
        public ICollection<FiscalYear> FiscalYears { get; set; } = new List<FiscalYear>();
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }

    public class Branch
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrganizationId { get; set; }
        [Required, MaxLength(150)]
        public string NameAr { get; set; } = string.Empty;
        [MaxLength(150)]
        public string NameEn { get; set; } = string.Empty;
        [MaxLength(20)]
        public string Code { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        [ForeignKey("OrganizationId")]
        public Organization? Organization { get; set; }
    }

    public class FiscalYear
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrganizationId { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsClosed { get; set; } = false;
        public bool IsActive { get; set; } = true;

        [ForeignKey("OrganizationId")]
        public Organization? Organization { get; set; }
    }

    public enum AccountType
    {
        Asset = 1,        // أصول
        Liability = 2,    // خصوم (التزامات)
        Equity = 3,       // حقوق ملكية
        Revenue = 4,      // إيرادات
        Expense = 5       // مصروفات
    }

    public enum AccountCategory
    {
        Header = 1,       // رئيسي / تجميعي
        Transactional = 2 // فرعي / تحليلي
    }

    public class Account
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrganizationId { get; set; }
        public Guid? ParentAccountId { get; set; }

        [Required, MaxLength(50)]
        public string Code { get; set; } = string.Empty;
        [Required, MaxLength(200)]
        public string NameAr { get; set; } = string.Empty;
        [MaxLength(200)]
        public string NameEn { get; set; } = string.Empty;

        public AccountType AccountType { get; set; }
        public AccountCategory AccountCategory { get; set; }
        public int Level { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        public decimal OpeningBalance { get; set; } = 0;
        public bool IsDebitNature { get; set; } = true; // طبيعة الحساب: مدين=true, دائن=false
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("OrganizationId")]
        public Organization? Organization { get; set; }

        [ForeignKey("ParentAccountId")]
        public Account? ParentAccount { get; set; }
        public ICollection<Account> SubAccounts { get; set; } = new List<Account>();
    }

    public class CostCenter
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrganizationId { get; set; }
        [Required, MaxLength(50)]
        public string Code { get; set; } = string.Empty;
        [Required, MaxLength(150)]
        public string NameAr { get; set; } = string.Empty;
        [MaxLength(150)]
        public string NameEn { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        [ForeignKey("OrganizationId")]
        public Organization? Organization { get; set; }
    }

    public enum DocumentType
    {
        JournalVoucher = 1,  // قيد يومية
        PaymentVoucher = 2,  // سند صرف
        ReceiptVoucher = 3   // سند قبض
    }

    public enum DocumentStatus
    {
        Draft = 1,   // مسودة
        Posted = 2,  // مرحل
        Canceled = 3 // ملغى
    }

    public class Document
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrganizationId { get; set; }
        public Guid BranchId { get; set; }
        public Guid FiscalYearId { get; set; }

        public DocumentType DocumentType { get; set; }
        public long DocumentNumber { get; set; }
        public DateTime DocumentDate { get; set; } = DateTime.Today;

        [MaxLength(100)]
        public string ReferenceNumber { get; set; } = string.Empty;
        [MaxLength(500)]
        public string Notes { get; set; } = string.Empty;

        public DocumentStatus Status { get; set; } = DocumentStatus.Draft;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalDebit { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCredit { get; set; }

        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("OrganizationId")]
        public Organization? Organization { get; set; }
        [ForeignKey("BranchId")]
        public Branch? Branch { get; set; }
        [ForeignKey("FiscalYearId")]
        public FiscalYear? FiscalYear { get; set; }
        [ForeignKey("CreatedByUserId")]
        public ApplicationUser? CreatedByUser { get; set; }

        public ICollection<DocumentDetails> Details { get; set; } = new List<DocumentDetails>();
    }

    public class DocumentDetails
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid DocumentId { get; set; }
        public Guid AccountId { get; set; }
        public Guid? CostCenterId { get; set; }

        public int RowIndex { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Debit { get; set; } = 0;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Credit { get; set; } = 0;

        [MaxLength(500)]
        public string LineNotes { get; set; } = string.Empty;

        [ForeignKey("DocumentId")]
        public Document? Document { get; set; }
        [ForeignKey("AccountId")]
        public Account? Account { get; set; }
        [ForeignKey("CostCenterId")]
        public CostCenter? CostCenter { get; set; }
    }

    public class ApplicationUser
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrganizationId { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? RoleId { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        [Required, MaxLength(150)]
        public string FullName { get; set; } = string.Empty;
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        public bool IsAdmin { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("OrganizationId")]
        public Organization? Organization { get; set; }
        [ForeignKey("BranchId")]
        public Branch? Branch { get; set; }
        [ForeignKey("RoleId")]
        public ApplicationRole? Role { get; set; }
    }

    public class ApplicationRole
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrganizationId { get; set; }
        [Required, MaxLength(100)]
        public string NameAr { get; set; } = string.Empty;
        [MaxLength(100)]
        public string NameEn { get; set; } = string.Empty;
        [MaxLength(250)]
        public string Description { get; set; } = string.Empty;

        [ForeignKey("OrganizationId")]
        public Organization? Organization { get; set; }

        public ICollection<RolePermission> Permissions { get; set; } = new List<RolePermission>();
    }

    public class RolePermission
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RoleId { get; set; }
        [Required, MaxLength(50)]
        public string Module { get; set; } = string.Empty; // Accounts, Documents, Reports, Users, Settings

        public bool CanView { get; set; } = true;
        public bool CanAdd { get; set; } = true;
        public bool CanEdit { get; set; } = true;
        public bool CanDelete { get; set; } = true;

        [ForeignKey("RoleId")]
        public ApplicationRole? Role { get; set; }
    }

    public class AuditLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrganizationId { get; set; }
        public Guid? UserId { get; set; }
        [MaxLength(100)]
        public string UserName { get; set; } = string.Empty;
        [MaxLength(100)]
        public string Action { get; set; } = string.Empty;
        [MaxLength(100)]
        public string Module { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}