using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AccountingERP.Data;
using AccountingERP.Models;

namespace AccountingERP.Services
{
    public interface IDocumentService
    {
        Task<List<DocumentDto>> GetDocumentsAsync(DocumentType? type = null, DocumentStatus? status = null, DateTime? fromDate = null, DateTime? toDate = null, string? search = null);
        Task<DocumentDto?> GetDocumentByIdAsync(Guid id);
        Task<ApiResponse<DocumentDto>> CreateDocumentAsync(CreateUpdateDocumentViewModel model);
        Task<ApiResponse<DocumentDto>> UpdateDocumentAsync(CreateUpdateDocumentViewModel model);
        Task<ApiResponse<bool>> DeleteDocumentAsync(Guid id);
        Task<ApiResponse<bool>> PostDocumentAsync(Guid id);
        Task<ApiResponse<bool>> UnpostDocumentAsync(Guid id);
        Task<long> GetNextDocumentNumberAsync(DocumentType type);
    }

    public class DocumentService : IDocumentService
    {
        private readonly AppDbContext _context;
        private readonly ITenantService _tenantService;

        public DocumentService(AppDbContext context, ITenantService tenantService)
        {
            _context = context;
            _tenantService = tenantService;
        }

        public async Task<List<DocumentDto>> GetDocumentsAsync(DocumentType? type = null, DocumentStatus? status = null, DateTime? fromDate = null, DateTime? toDate = null, string? search = null)
        {
            var query = _context.Documents
                .Include(d => d.CreatedByUser)
                .Include(d => d.Branch)
                .Include(d => d.Details)
                    .ThenInclude(dd => dd.Account)
                .Include(d => d.Details)
                    .ThenInclude(dd => dd.CostCenter)
                .AsQueryable();

            if (type.HasValue)
                query = query.Where(d => d.DocumentType == type.Value);

            if (status.HasValue)
                query = query.Where(d => d.Status == status.Value);

            if (fromDate.HasValue)
                query = query.Where(d => d.DocumentDate >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(d => d.DocumentDate <= toDate.Value.Date.AddDays(1).AddTicks(-1));

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                query = query.Where(d => d.DocumentNumber.ToString().Contains(s) ||
                                         d.Notes.Contains(s) ||
                                         d.ReferenceNumber.Contains(s));
            }

            var list = await query.OrderByDescending(d => d.DocumentDate).ThenByDescending(d => d.DocumentNumber).ToListAsync();
            return list.Select(MapToDto).ToList();
        }

        public async Task<DocumentDto?> GetDocumentByIdAsync(Guid id)
        {
            var doc = await _context.Documents
                .Include(d => d.CreatedByUser)
                .Include(d => d.Branch)
                .Include(d => d.Details)
                    .ThenInclude(dd => dd.Account)
                .Include(d => d.Details)
                    .ThenInclude(dd => dd.CostCenter)
                .FirstOrDefaultAsync(d => d.Id == id);

            return doc != null ? MapToDto(doc) : null;
        }

        public async Task<ApiResponse<DocumentDto>> CreateDocumentAsync(CreateUpdateDocumentViewModel model)
        {
            var response = new ApiResponse<DocumentDto>();

            // Validations
            var valResult = ValidateDocumentModel(model);
            if (!valResult.Success) return valResult;

            // Database Transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var activeFiscalYear = await _context.FiscalYears
                    .FirstOrDefaultAsync(f => f.IsActive && !f.IsClosed);

                if (activeFiscalYear == null)
                {
                    activeFiscalYear = new FiscalYear
                    {
                        Id = Guid.NewGuid(),
                        OrganizationId = _tenantService.OrganizationId,
                        Name = $"السنة المالية {DateTime.Today.Year}",
                        StartDate = new DateTime(DateTime.Today.Year, 1, 1),
                        EndDate = new DateTime(DateTime.Today.Year, 12, 31),
                        IsActive = true
                    };
                    _context.FiscalYears.Add(activeFiscalYear);
                    await _context.SaveChangesAsync();
                }

                var branch = await _context.Branches.FirstOrDefaultAsync();
                if (branch == null)
                {
                    branch = new Branch
                    {
                        Id = Guid.NewGuid(),
                        OrganizationId = _tenantService.OrganizationId,
                        NameAr = "الفرع الرئيسي",
                        Code = "MAIN"
                    };
                    _context.Branches.Add(branch);
                    await _context.SaveChangesAsync();
                }

                long docNum = model.DocumentNumber > 0 ? model.DocumentNumber : await GetNextDocumentNumberAsync(model.DocumentType);

                var document = new Document
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = _tenantService.OrganizationId,
                    BranchId = branch.Id,
                    FiscalYearId = activeFiscalYear.Id,
                    DocumentType = model.DocumentType,
                    DocumentNumber = docNum,
                    DocumentDate = model.DocumentDate,
                    ReferenceNumber = model.ReferenceNumber?.Trim() ?? string.Empty,
                    Notes = model.Notes?.Trim() ?? string.Empty,
                    Status = model.Status,
                    TotalDebit = model.Details.Sum(d => d.Debit),
                    TotalCredit = model.Details.Sum(d => d.Credit),
                    CreatedByUserId = _tenantService.UserId != Guid.Empty ? _tenantService.UserId : (await _context.Users.Select(u => u.Id).FirstOrDefaultAsync()),
                    CreatedAt = DateTime.UtcNow
                };

                int index = 1;
                foreach (var item in model.Details)
                {
                    document.Details.Add(new DocumentDetails
                    {
                        Id = Guid.NewGuid(),
                        DocumentId = document.Id,
                        AccountId = item.AccountId,
                        CostCenterId = item.CostCenterId,
                        Debit = item.Debit,
                        Credit = item.Credit,
                        LineNotes = item.LineNotes?.Trim() ?? string.Empty,
                        RowIndex = index++
                    });
                }

                _context.Documents.Add(document);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                response.Success = true;
                response.Message = "تم حفظ المستند المحاسبي بنجاح";
                response.Data = await GetDocumentByIdAsync(document.Id);
                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.Success = false;
                response.Message = "حدث خطأ أثناء حفظ القيد المحاسبي: " + ex.Message;
                return response;
            }
        }

        public async Task<ApiResponse<DocumentDto>> UpdateDocumentAsync(CreateUpdateDocumentViewModel model)
        {
            var response = new ApiResponse<DocumentDto>();

            if (!model.Id.HasValue)
            {
                response.Success = false;
                response.Message = "معرف المستند غير صحيح";
                return response;
            }

            var valResult = ValidateDocumentModel(model);
            if (!valResult.Success) return valResult;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var doc = await _context.Documents
                    .Include(d => d.Details)
                    .FirstOrDefaultAsync(d => d.Id == model.Id.Value);

                if (doc == null)
                {
                    response.Success = false;
                    response.Message = "المستند المحاسبي غير موجود";
                    return response;
                }

                if (doc.Status == DocumentStatus.Posted)
                {
                    response.Success = false;
                    response.Message = "لا يمكن تعديل مستند مرحل. يرجى إلغاء الترحيل أولاً";
                    return response;
                }

                doc.DocumentDate = model.DocumentDate;
                doc.ReferenceNumber = model.ReferenceNumber?.Trim() ?? string.Empty;
                doc.Notes = model.Notes?.Trim() ?? string.Empty;
                doc.TotalDebit = model.Details.Sum(d => d.Debit);
                doc.TotalCredit = model.Details.Sum(d => d.Credit);

                // Remove old details
                _context.DocumentDetails.RemoveRange(doc.Details);

                // Add new details
                int index = 1;
                foreach (var item in model.Details)
                {
                    _context.DocumentDetails.Add(new DocumentDetails
                    {
                        Id = Guid.NewGuid(),
                        DocumentId = doc.Id,
                        AccountId = item.AccountId,
                        CostCenterId = item.CostCenterId,
                        Debit = item.Debit,
                        Credit = item.Credit,
                        LineNotes = item.LineNotes?.Trim() ?? string.Empty,
                        RowIndex = index++
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                response.Success = true;
                response.Message = "تم تحديث المستند المحاسبي بنجاح";
                response.Data = await GetDocumentByIdAsync(doc.Id);
                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.Success = false;
                response.Message = "حدث خطأ أثناء تعديل المستند: " + ex.Message;
                return response;
            }
        }

        public async Task<ApiResponse<bool>> DeleteDocumentAsync(Guid id)
        {
            var response = new ApiResponse<bool>();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var doc = await _context.Documents.FindAsync(id);
                if (doc == null)
                {
                    response.Success = false;
                    response.Message = "المستند غير موجود";
                    return response;
                }

                if (doc.Status == DocumentStatus.Posted)
                {
                    response.Success = false;
                    response.Message = "لا يمكن حذف مستند مرحل";
                    return response;
                }

                _context.Documents.Remove(doc);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                response.Success = true;
                response.Data = true;
                response.Message = "تم حذف المستند بنجاح";
                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.Success = false;
                response.Message = "حدث خطأ أثناء حذف المستند: " + ex.Message;
                return response;
            }
        }

        public async Task<ApiResponse<bool>> PostDocumentAsync(Guid id)
        {
            var response = new ApiResponse<bool>();
            var doc = await _context.Documents.FindAsync(id);
            if (doc == null)
            {
                response.Success = false;
                response.Message = "المستند غير موجود";
                return response;
            }

            if (doc.Status == DocumentStatus.Posted)
            {
                response.Success = false;
                response.Message = "المستند مرحل بالفعل";
                return response;
            }

            doc.Status = DocumentStatus.Posted;
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Data = true;
            response.Message = "تم ترحيل المستند المحاسبي بنجاح إلى الحسابات العامة";
            return response;
        }

        public async Task<ApiResponse<bool>> UnpostDocumentAsync(Guid id)
        {
            var response = new ApiResponse<bool>();
            var doc = await _context.Documents.FindAsync(id);
            if (doc == null)
            {
                response.Success = false;
                response.Message = "المستند غير موجود";
                return response;
            }

            doc.Status = DocumentStatus.Draft;
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Data = true;
            response.Message = "تم إلغاء ترحيل المستند وإعادته إلى وضع المسودة";
            return response;
        }

        public async Task<long> GetNextDocumentNumberAsync(DocumentType type)
        {
            var maxNum = await _context.Documents
                .Where(d => d.DocumentType == type)
                .MaxAsync(d => (long?)d.DocumentNumber) ?? 0;

            return maxNum + 1;
        }

        private static ApiResponse<DocumentDto> ValidateDocumentModel(CreateUpdateDocumentViewModel model)
        {
            var res = new ApiResponse<DocumentDto>();

            if (model.Details == null || model.Details.Count == 0)
            {
                res.Success = false;
                res.Message = "يجب إضافة سطر واحد على الأقل في القيد المحاسبي";
                return res;
            }

            var totalDebit = model.Details.Sum(d => d.Debit);
            var totalCredit = model.Details.Sum(d => d.Credit);

            if (Math.Abs(totalDebit - totalCredit) > 0.001m)
            {
                res.Success = false;
                res.Message = $"القيد المحاسبي غير متوازن! مجموع المدين ({totalDebit:N2}) لا يساوي مجموع الدائن ({totalCredit:N2})";
                return res;
            }

            if (totalDebit <= 0)
            {
                res.Success = false;
                res.Message = "يجب أن تكون قيمة القيد أكبر من صفر";
                return res;
            }

            foreach (var detail in model.Details)
            {
                if (detail.AccountId == Guid.Empty)
                {
                    res.Success = false;
                    res.Message = "توجد أسطر لم يتم تحديد الحساب لها";
                    return res;
                }
            }

            res.Success = true;
            return res;
        }

        private static DocumentDto MapToDto(Document d)
        {
            return new DocumentDto
            {
                Id = d.Id,
                DocumentType = d.DocumentType,
                DocumentTypeTitle = GetDocumentTypeTitle(d.DocumentType),
                DocumentNumber = d.DocumentNumber,
                DocumentDate = d.DocumentDate,
                DocumentDateFormatted = d.DocumentDate.ToString("yyyy-MM-dd"),
                ReferenceNumber = d.ReferenceNumber,
                Notes = d.Notes,
                Status = d.Status,
                StatusTitle = d.Status == DocumentStatus.Posted ? "مرحل" : (d.Status == DocumentStatus.Draft ? "مسودة" : "ملغى"),
                TotalDebit = d.TotalDebit,
                TotalCredit = d.TotalCredit,
                CreatedByUserName = d.CreatedByUser?.FullName ?? "النظام",
                BranchName = d.Branch?.NameAr ?? "الفرع الرئيسي",
                Details = d.Details.Select(dd => new DocumentDetailsDto
                {
                    Id = dd.Id,
                    AccountId = dd.AccountId,
                    AccountCode = dd.Account?.Code ?? string.Empty,
                    AccountName = dd.Account?.NameAr ?? string.Empty,
                    CostCenterId = dd.CostCenterId,
                    CostCenterName = dd.CostCenter?.NameAr ?? string.Empty,
                    Debit = dd.Debit,
                    Credit = dd.Credit,
                    LineNotes = dd.LineNotes
                }).ToList()
            };
        }

        private static string GetDocumentTypeTitle(DocumentType type) => type switch
        {
            DocumentType.JournalVoucher => "قيد يومية",
            DocumentType.PaymentVoucher => "سند صرف",
            DocumentType.ReceiptVoucher => "سند قبض",
            _ => "مستند"
        };
    }
}