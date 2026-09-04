using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AccountingERP.Data;
using AccountingERP.Models;

namespace AccountingERP.Services
{
    public interface IAccountService
    {
        Task<List<AccountDto>> GetAllAccountsAsync();
        Task<List<AccountDto>> GetAccountTreeAsync();
        Task<List<AccountDto>> GetTransactionalAccountsAsync();
        Task<AccountDto?> GetAccountByIdAsync(Guid id);
        Task<AccountDto?> GetAccountByCodeAsync(string code);
        Task<ApiResponse<AccountDto>> CreateAccountAsync(CreateUpdateAccountViewModel model);
        Task<ApiResponse<AccountDto>> UpdateAccountAsync(CreateUpdateAccountViewModel model);
        Task<ApiResponse<bool>> DeleteAccountAsync(Guid id);
        Task<string> GenerateNextAccountCodeAsync(Guid? parentAccountId, AccountType accountType);
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

        public async Task<List<AccountDto>> GetAllAccountsAsync()
        {
            var accounts = await _context.Accounts
                .Include(a => a.ParentAccount)
                .OrderBy(a => a.Code)
                .ToListAsync();

            return accounts.Select(MapToDto).ToList();
        }

        public async Task<List<AccountDto>> GetAccountTreeAsync()
        {
            var allAccounts = await _context.Accounts
                .Include(a => a.ParentAccount)
                .OrderBy(a => a.Code)
                .ToListAsync();

            var dtos = allAccounts.Select(MapToDto).ToList();
            var lookup = dtos.ToDictionary(a => a.Id);

            var rootNodes = new List<AccountDto>();
            foreach (var account in dtos)
            {
                if (account.ParentAccountId.HasValue && lookup.TryGetValue(account.ParentAccountId.Value, out var parent))
                {
                    parent.SubAccounts.Add(account);
                }
                else
                {
                    rootNodes.Add(account);
                }
            }

            return rootNodes;
        }

        public async Task<List<AccountDto>> GetTransactionalAccountsAsync()
        {
            var accounts = await _context.Accounts
                .Where(a => a.AccountCategory == AccountCategory.Transactional && a.IsActive)
                .OrderBy(a => a.Code)
                .ToListAsync();

            return accounts.Select(MapToDto).ToList();
        }

        public async Task<AccountDto?> GetAccountByIdAsync(Guid id)
        {
            var account = await _context.Accounts
                .Include(a => a.ParentAccount)
                .FirstOrDefaultAsync(a => a.Id == id);

            return account != null ? MapToDto(account) : null;
        }

        public async Task<AccountDto?> GetAccountByCodeAsync(string code)
        {
            var account = await _context.Accounts
                .Include(a => a.ParentAccount)
                .FirstOrDefaultAsync(a => a.Code == code);

            return account != null ? MapToDto(account) : null;
        }

        public async Task<ApiResponse<AccountDto>> CreateAccountAsync(CreateUpdateAccountViewModel model)
        {
            var response = new ApiResponse<AccountDto>();

            // Check if code exists
            var existingCode = await _context.Accounts.AnyAsync(a => a.Code == model.Code);
            if (existingCode)
            {
                response.Success = false;
                response.Message = "كود الحساب موجود بالفعل في النظام";
                return response;
            }

            int level = 1;
            if (model.ParentAccountId.HasValue)
            {
                var parent = await _context.Accounts.FindAsync(model.ParentAccountId.Value);
                if (parent == null)
                {
                    response.Success = false;
                    response.Message = "الحساب الرئيسي المختار غير موجود";
                    return response;
                }
                level = parent.Level + 1;

                // Parent account must be converted to Header if it was Transactional and had no postings
                if (parent.AccountCategory == AccountCategory.Transactional)
                {
                    parent.AccountCategory = AccountCategory.Header;
                }
            }

            var account = new Account
            {
                Id = Guid.NewGuid(),
                OrganizationId = _tenantService.OrganizationId,
                ParentAccountId = model.ParentAccountId,
                Code = model.Code.Trim(),
                NameAr = model.NameAr.Trim(),
                NameEn = model.NameEn?.Trim() ?? string.Empty,
                AccountType = model.AccountType,
                AccountCategory = model.AccountCategory,
                Level = level,
                OpeningBalance = model.OpeningBalance,
                IsDebitNature = model.IsDebitNature,
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "تم إضافة الحساب بنجاح";
            response.Data = MapToDto(account);
            return response;
        }

        public async Task<ApiResponse<AccountDto>> UpdateAccountAsync(CreateUpdateAccountViewModel model)
        {
            var response = new ApiResponse<AccountDto>();

            if (!model.Id.HasValue)
            {
                response.Success = false;
                response.Message = "معرف الحساب غير صحيح";
                return response;
            }

            var account = await _context.Accounts.FindAsync(model.Id.Value);
            if (account == null)
            {
                response.Success = false;
                response.Message = "الحساب غير موجود";
                return response;
            }

            var existingCode = await _context.Accounts.AnyAsync(a => a.Code == model.Code && a.Id != model.Id.Value);
            if (existingCode)
            {
                response.Success = false;
                response.Message = "كود الحساب مستخدم من قبل حساب آخر";
                return response;
            }

            account.Code = model.Code.Trim();
            account.NameAr = model.NameAr.Trim();
            account.NameEn = model.NameEn?.Trim() ?? string.Empty;
            account.AccountType = model.AccountType;
            account.AccountCategory = model.AccountCategory;
            account.OpeningBalance = model.OpeningBalance;
            account.IsDebitNature = model.IsDebitNature;
            account.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "تم تعديل بيانات الحساب بنجاح";
            response.Data = MapToDto(account);
            return response;
        }

        public async Task<ApiResponse<bool>> DeleteAccountAsync(Guid id)
        {
            var response = new ApiResponse<bool>();

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                response.Success = false;
                response.Message = "الحساب غير موجود";
                return response;
            }

            // Check if account has sub-accounts
            var hasSubAccounts = await _context.Accounts.AnyAsync(a => a.ParentAccountId == id);
            if (hasSubAccounts)
            {
                response.Success = false;
                response.Message = "لا يمكن حذف هذا الحساب لانه يحتوي على حسابات فرعية مرتبطة به";
                return response;
            }

            // Check if account has transactions
            var hasTransactions = await _context.DocumentDetails.AnyAsync(dd => dd.AccountId == id);
            if (hasTransactions)
            {
                response.Success = false;
                response.Message = "لا يمكن حذف الحساب لانه توجد قيود وسندات محاسبية مسجلة عليه";
                return response;
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Data = true;
            response.Message = "تم حذف الحساب بنجاح";
            return response;
        }

        public async Task<string> GenerateNextAccountCodeAsync(Guid? parentAccountId, AccountType accountType)
        {
            if (parentAccountId.HasValue)
            {
                var parent = await _context.Accounts.FindAsync(parentAccountId.Value);
                if (parent != null)
                {
                    var childCount = await _context.Accounts.CountAsync(a => a.ParentAccountId == parentAccountId.Value);
                    return $"{parent.Code}{(childCount + 1):D2}";
                }
            }

            var typePrefix = ((int)accountType).ToString();
            var rootCount = await _context.Accounts.CountAsync(a => a.ParentAccountId == null && a.AccountType == accountType);
            return $"{typePrefix}{(rootCount + 1):D3}";
        }

        private static AccountDto MapToDto(Account a)
        {
            return new AccountDto
            {
                Id = a.Id,
                Code = a.Code,
                NameAr = a.NameAr,
                NameEn = a.NameEn,
                ParentAccountId = a.ParentAccountId,
                ParentAccountCode = a.ParentAccount?.Code ?? string.Empty,
                ParentAccountName = a.ParentAccount?.NameAr ?? string.Empty,
                AccountType = a.AccountType,
                AccountTypeTitle = GetAccountTypeTitle(a.AccountType),
                AccountCategory = a.AccountCategory,
                AccountCategoryTitle = a.AccountCategory == AccountCategory.Header ? "رئيسي" : "فرعي/تحليلي",
                Level = a.Level,
                OpeningBalance = a.OpeningBalance,
                IsDebitNature = a.IsDebitNature,
                IsActive = a.IsActive
            };
        }

        private static string GetAccountTypeTitle(AccountType type) => type switch
        {
            AccountType.Asset => "أصول",
            AccountType.Liability => "خصوم",
            AccountType.Equity => "حقوق ملكية",
            AccountType.Revenue => "إيرادات",
            AccountType.Expense => "مصروفات",
            _ => "غير محدد"
        };
    }
}