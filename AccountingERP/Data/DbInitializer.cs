using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AccountingERP.Models;

namespace AccountingERP.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            if (await context.Organizations.AnyAsync())
            {
                return; // Already seeded
            }

            // 1. Create Default Organization
            var org = new Organization
            {
                Id = Guid.NewGuid(),
                NameAr = "مؤسسة الحلول المحاسبية المتقدمة",
                NameEn = "Advanced Accounting Solutions Est.",
                TaxNumber = "310123456700003",
                CommercialRegistration = "1010987654",
                Address = "الرياض - المملكة العربية السعودية",
                Phone = "+966 11 456 7890",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Organizations.Add(org);

            // 2. Create Default Branch
            var branch = new Branch
            {
                Id = Guid.NewGuid(),
                OrganizationId = org.Id,
                NameAr = "الفرع الرئيسي - الرياض",
                NameEn = "Main Branch - Riyadh",
                Code = "BR01",
                IsActive = true
            };
            context.Branches.Add(branch);

            // 3. Create Default Fiscal Year
            var fiscalYear = new FiscalYear
            {
                Id = Guid.NewGuid(),
                OrganizationId = org.Id,
                Name = "السنة المالية 2026",
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2026, 12, 31),
                IsActive = true,
                IsClosed = false
            };
            context.FiscalYears.Add(fiscalYear);

            // 4. Create Default Admin User
            var adminUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                OrganizationId = org.Id,
                BranchId = branch.Id,
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123456"),
                FullName = "مدير النظام",
                Email = "admin@accounting-system.com",
                Phone = "0501234567",
                IsAdmin = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Users.Add(adminUser);

            // 5. Create Cost Centers
            var cc1 = new CostCenter { Id = Guid.NewGuid(), OrganizationId = org.Id, Code = "CC101", NameAr = "الإدارة العامة", NameEn = "Administration" };
            var cc2 = new CostCenter { Id = Guid.NewGuid(), OrganizationId = org.Id, Code = "CC102", NameAr = "المشاريع التشغيلية", NameEn = "Projects" };
            context.CostCenters.AddRange(cc1, cc2);

            // 6. Build Standard Chart of Accounts Tree (دليل الحسابات الموحد)
            // Level 1: Main Headers
            var assets = new Account { Id = Guid.NewGuid(), OrganizationId = org.Id, Code = "1", NameAr = "الأصول", AccountType = AccountType.Asset, AccountCategory = AccountCategory.Header, Level = 1, IsDebitNature = true };
            var liabilities = new Account { Id = Guid.NewGuid(), OrganizationId = org.Id, Code = "2", NameAr = "الخصوم (الالتزامات)", AccountType = AccountType.Liability, AccountCategory = AccountCategory.Header, Level = 1, IsDebitNature = false };
            var equity = new Account { Id = Guid.NewGuid(), OrganizationId = org.Id, Code = "3", NameAr = "حقوق الملكية", AccountType = AccountType.Equity, AccountCategory = AccountCategory.Header, Level = 1, IsDebitNature = false };
            var revenues = new Account { Id = Guid.NewGuid(), OrganizationId = org.Id, Code = "4", NameAr = "الإيرادات", AccountType = AccountType.Revenue, AccountCategory = AccountCategory.Header, Level = 1, IsDebitNature = false };
            var expenses = new Account { Id = Guid.NewGuid(), OrganizationId = org.Id, Code = "5", NameAr = "المصروفات", AccountType = AccountType.Expense, AccountCategory = AccountCategory.Header, Level = 1, IsDebitNature = true };

            context.Accounts.AddRange(assets, liabilities, equity, revenues, expenses);

            // Level 2: Sub Headers
            var currentAssets = new Account { Id = Guid.NewGuid(), OrganizationId = org.Id, ParentAccountId = assets.Id, Code = "11", NameAr = "الأصول المتداولة", AccountType = AccountType.Asset, AccountCategory = AccountCategory.Header, Level = 2, IsDebitNature = true };
            var fixedAssets = new Account { Id = Guid.NewGuid(), OrganizationId = org.Id, ParentAccountId = assets.Id, Code = "12", NameAr = "الأصول الثابتة", AccountType = AccountType.Asset, AccountCategory = AccountCategory.Header, Level = 2, IsDebitNature = true };

            var currentLiabilities = new Account { Id = Guid.NewGuid(), OrganizationId = org.Id, ParentAccountId = liabilities.Id, Code = "21", NameAr = "الخصوم المتداولة", AccountType = AccountType.Liability, AccountCategory = AccountCategory.Header, Level = 2, IsDebitNature = false };

            var capitalAcc = new Account { Id = Guid.NewGuid(), OrganizationId = org.Id, ParentAccountId = equity.Id, Code = "31", NameAr = "رأس المال", AccountType = AccountType.Equity, AccountCategory = AccountCategory.Transactional, Level = 2, IsDebitNature = false, OpeningBalance = 500000 };

            var salesRevenues = new Account { Id = Guid.NewGuid(), OrganizationId = org.Id, ParentAccountId = revenues.Id, Code = "41", NameAr = "إيرادات المبيعات والخدمات", AccountType = AccountType.Revenue, AccountCategory = AccountCategory.Header, Level = 2, IsDebitNature = false };

            var adminExpenses = new Account { Id = Guid.NewGuid(), OrganizationId = org.Id, ParentAccountId = expenses.Id, Code = "51", NameAr = "المصروفات العمومية والإدارية", AccountType = AccountType.Expense, AccountCategory = AccountCategory.Header, Level = 2, IsDebitNature = true };

            context.Accounts.AddRange(currentAssets, fixedAssets, currentLiabilities, capitalAcc, salesRevenues, adminExpenses);

            // Level 3: Transactional Accounts (الحسابات الفرعية التحليلية)
            var cashAcc = new Account { Id = Guid.NewGuid(), OrganizationId = org.Id, ParentAccountId = currentAssets.Id, Code = "1101", NameAr = "الصندوق / الصندوق الرئيسي", AccountType = AccountType.Asset, AccountCategory = AccountCategory.Transactional, Level = 3, IsDebitNature = true, OpeningBalance = 50000 };
            var bankAcc = new Account { Id = Guid.NewGuid(), OrganizationId = org.Id, ParentAccountId = currentAssets.Id, Code = "1102", NameAr = "البنك - مصرف الراجحي", AccountType = AccountType.Asset, AccountCategory = AccountCategory.Transactional, Level = 3, IsDebitNature = true, OpeningBalance = 250000 };
            var customersAcc = new Account { Id = Guid.NewGuid(), OrganizationId = org.Id, ParentAccountId = currentAssets.Id, Code = "1103", NameAr = "العملاء - شركة الأمل للتجارة", AccountType = AccountType.Asset, AccountCategory = AccountCategory.Transactional, Level = 3, IsDebitNature = true, OpeningBalance = 20000 };

            var suppliersAcc = new Account { Id = Guid.NewGuid(), OrganizationId = org.Id, ParentAccountId = currentLiabilities.Id, Code = "2101", NameAr = "الموردون - شركة التقنية الحديثة", AccountType = AccountType.Liability, AccountCategory = AccountCategory.Transactional, Level = 3, IsDebitNature = false, OpeningBalance = 15000 };

            var mainSalesAcc = new Account { Id = Guid.NewGuid(), OrganizationId = org.Id, ParentAccountId = salesRevenues.Id, Code = "4101", NameAr = "إيرادات الخدمات البرمجية والاستشارات", AccountType = AccountType.Revenue, AccountCategory = AccountCategory.Transactional, Level = 3, IsDebitNature = false };

            var rentExpAcc = new Account { Id = Guid.NewGuid(), OrganizationId = org.Id, ParentAccountId = adminExpenses.Id, Code = "5101", NameAr = "مصروف الإيجار", AccountType = AccountType.Expense, AccountCategory = AccountCategory.Transactional, Level = 3, IsDebitNature = true };
            var salaryExpAcc = new Account { Id = Guid.NewGuid(), OrganizationId = org.Id, ParentAccountId = adminExpenses.Id, Code = "5102", NameAr = "مصروف الرواتب والأجور", AccountType = AccountType.Expense, AccountCategory = AccountCategory.Transactional, Level = 3, IsDebitNature = true };
            var utilExpAcc = new Account { Id = Guid.NewGuid(), OrganizationId = org.Id, ParentAccountId = adminExpenses.Id, Code = "5103", NameAr = "مصروف الكهرباء والمياه", AccountType = AccountType.Expense, AccountCategory = AccountCategory.Transactional, Level = 3, IsDebitNature = true };

            context.Accounts.AddRange(cashAcc, bankAcc, customersAcc, suppliersAcc, mainSalesAcc, rentExpAcc, salaryExpAcc, utilExpAcc);

            await context.SaveChangesAsync();

            // 7. Seed Sample Accounting Journal Documents & Details
            // Document 1: Initial Capital Injection
            var doc1 = new Document
            {
                Id = Guid.NewGuid(),
                OrganizationId = org.Id,
                BranchId = branch.Id,
                FiscalYearId = fiscalYear.Id,
                DocumentType = DocumentType.JournalVoucher,
                DocumentNumber = 1001,
                DocumentDate = new DateTime(2026, 1, 15),
                ReferenceNumber = "REF-2026-001",
                Notes = "قيد إيداع رأس المال الأولي في حساب البنك والصندوق",
                Status = DocumentStatus.Posted,
                TotalDebit = 300000,
                TotalCredit = 300000,
                CreatedByUserId = adminUser.Id,
                CreatedAt = DateTime.UtcNow
            };

            doc1.Details.Add(new DocumentDetails { Id = Guid.NewGuid(), DocumentId = doc1.Id, AccountId = bankAcc.Id, CostCenterId = cc1.Id, Debit = 200000, Credit = 0, LineNotes = "إيداع في البنك", RowIndex = 1 });
            doc1.Details.Add(new DocumentDetails { Id = Guid.NewGuid(), DocumentId = doc1.Id, AccountId = cashAcc.Id, CostCenterId = cc1.Id, Debit = 100000, Credit = 0, LineNotes = "إيداع في الصندوق", RowIndex = 2 });
            doc1.Details.Add(new DocumentDetails { Id = Guid.NewGuid(), DocumentId = doc1.Id, AccountId = capitalAcc.Id, CostCenterId = cc1.Id, Debit = 0, Credit = 300000, LineNotes = "رأس المال", RowIndex = 3 });

            // Document 2: Revenue Received
            var doc2 = new Document
            {
                Id = Guid.NewGuid(),
                OrganizationId = org.Id,
                BranchId = branch.Id,
                FiscalYearId = fiscalYear.Id,
                DocumentType = DocumentType.ReceiptVoucher,
                DocumentNumber = 2001,
                DocumentDate = new DateTime(2026, 2, 1),
                ReferenceNumber = "INV-0042",
                Notes = "تحصيل إيراد خدمات تطوير أنظمة من شركة الأمل",
                Status = DocumentStatus.Posted,
                TotalDebit = 45000,
                TotalCredit = 45000,
                CreatedByUserId = adminUser.Id,
                CreatedAt = DateTime.UtcNow
            };

            doc2.Details.Add(new DocumentDetails { Id = Guid.NewGuid(), DocumentId = doc2.Id, AccountId = bankAcc.Id, CostCenterId = cc2.Id, Debit = 45000, Credit = 0, LineNotes = "تحصيل عبر التحويل البنكي", RowIndex = 1 });
            doc2.Details.Add(new DocumentDetails { Id = Guid.NewGuid(), DocumentId = doc2.Id, AccountId = mainSalesAcc.Id, CostCenterId = cc2.Id, Debit = 0, Credit = 45000, LineNotes = "إيراد خدمات استشارية", RowIndex = 2 });

            // Document 3: Payment Voucher for Office Rent
            var doc3 = new Document
            {
                Id = Guid.NewGuid(),
                OrganizationId = org.Id,
                BranchId = branch.Id,
                FiscalYearId = fiscalYear.Id,
                DocumentType = DocumentType.PaymentVoucher,
                DocumentNumber = 3001,
                DocumentDate = new DateTime(2026, 2, 10),
                ReferenceNumber = "CHK-9812",
                Notes = "سداد قيمة إيجار المكاتب السنوي",
                Status = DocumentStatus.Posted,
                TotalDebit = 25000,
                TotalCredit = 25000,
                CreatedByUserId = adminUser.Id,
                CreatedAt = DateTime.UtcNow
            };

            doc3.Details.Add(new DocumentDetails { Id = Guid.NewGuid(), DocumentId = doc3.Id, AccountId = rentExpAcc.Id, CostCenterId = cc1.Id, Debit = 25000, Credit = 0, LineNotes = "إيجار المكتب الرئيسية", RowIndex = 1 });
            doc3.Details.Add(new DocumentDetails { Id = Guid.NewGuid(), DocumentId = doc3.Id, AccountId = bankAcc.Id, CostCenterId = cc1.Id, Debit = 0, Credit = 25000, LineNotes = "شيك مسحوب على البنك", RowIndex = 2 });

            context.Documents.AddRange(doc1, doc2, doc3);
            await context.SaveChangesAsync();
        }
    }
}