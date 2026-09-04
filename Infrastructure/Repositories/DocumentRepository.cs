using Domain.DTOs;
using Domain.Entities;
using Domain.Entities.Base;
using Shared.Constant.Roles;

namespace Infrastructure.Repositories
{
    public class DocumentRepository : GenericRepository<Document>
    {
        public DocumentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<Result<Document>> AddAsync(Document entity)
        {
            var total = ((entity.Item1Quantity ?? 0) * (entity.Item1Price ?? 0)) +
                        ((entity.Item2Quantity ?? 0) * (entity.Item2Price ?? 0)) +
                        ((entity.Item3Quantity ?? 0) * (entity.Item3Price ?? 0));
            entity.FinalTotal = total;

            decimal exchangeRate = 1;
            if (entity.CurrencyId.HasValue && entity.CurrencyId.Value != Guid.Empty)
            {
                var currency = await _context.Currencies.FindAsync(entity.CurrencyId.Value);
                if (currency?.CurrentExchangeRate != null && currency.CurrentExchangeRate > 0)
                {
                    exchangeRate = currency.CurrentExchangeRate.Value;
                }
            }
            entity.ExchangeRate = exchangeRate;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await this._context.Documents.AddAsync(entity);
                DocumentDetail detail = new DocumentDetail();
                if (entity.TypeId == 1)
                {
                    detail.Debit = total;
                    detail.Credit = 0;
                    detail.Item1QuantityDebit = entity.Item1Quantity ?? 0;
                    detail.Item2QuantityDebit = entity.Item2Quantity ?? 0;
                    detail.Item3QuantityDebit = entity.Item3Quantity ?? 0;
                    detail.Item1QuantityCredit = 0;
                    detail.Item2QuantityCredit = 0;
                    detail.Item3QuantityCredit = 0;
                    detail.AccountId = entity.AccountId;
                    detail.CurrencyId = entity.CurrencyId;
                    detail.LocalDebit = total * exchangeRate;
                    detail.LocalCredit = 0;
                }
                else if (entity.TypeId == 2)
                {
                    detail.Credit = total;
                    detail.Debit = 0;
                    detail.Item1QuantityDebit = 0;
                    detail.Item2QuantityDebit = 0;
                    detail.Item3QuantityDebit = 0;
                    detail.Item1QuantityCredit = entity.Item1Quantity ?? 0;
                    detail.Item2QuantityCredit = entity.Item2Quantity ?? 0;
                    detail.Item3QuantityCredit = entity.Item3Quantity ?? 0;
                    detail.AccountId = entity.AccountId;
                    detail.CurrencyId = entity.CurrencyId;
                    detail.LocalDebit = 0;
                    detail.LocalCredit = total * exchangeRate;
                }

                detail.DocumentId = entity.Id;
                detail.UserId = entity.UserId;
                detail.ExchangeRate = entity.ExchangeRate;
                detail.OrganizationId = entity.OrganizationId;
                detail.Description = entity.Description;

                await _context.DocumentDetails.AddAsync(detail);
                await this._context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Result.Success<Document>(entity);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Result.Failure<Document>(new Error(ex.Message));
            }
        }

        public override async Task<Result<Document>> UpdateAsync(Guid id, Document entity)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingDoc = await _context.Documents.FindAsync(id);
                if (existingDoc == null)
                {
                    return Result.Failure<Document>(new Error("السند غير موجود"));
                }

                var total = ((entity.Item1Quantity ?? 0) * (entity.Item1Price ?? 0)) +
                            ((entity.Item2Quantity ?? 0) * (entity.Item2Price ?? 0)) +
                            ((entity.Item3Quantity ?? 0) * (entity.Item3Price ?? 0));
                
                existingDoc.Item1Quantity = entity.Item1Quantity;
                existingDoc.Item1Price = entity.Item1Price;
                existingDoc.Item2Quantity = entity.Item2Quantity;
                existingDoc.Item2Price = entity.Item2Price;
                existingDoc.Item3Quantity = entity.Item3Quantity;
                existingDoc.Item3Price = entity.Item3Price;
                existingDoc.FinalTotal = total;
                existingDoc.AccountId = entity.AccountId;
                existingDoc.CurrencyId = entity.CurrencyId;
                existingDoc.Description = entity.Description;
                existingDoc.ModifiedDate = DateTime.Now;

                decimal exchangeRate = 1;
                if (entity.CurrencyId.HasValue && entity.CurrencyId.Value != Guid.Empty)
                {
                    var currency = await _context.Currencies.FindAsync(entity.CurrencyId.Value);
                    if (currency?.CurrentExchangeRate != null && currency.CurrentExchangeRate > 0)
                    {
                        exchangeRate = currency.CurrentExchangeRate.Value;
                    }
                }
                existingDoc.ExchangeRate = exchangeRate;

                // Update related DocumentDetail
                var existingDetails = _context.DocumentDetails.Where(d => d.DocumentId == id).ToList();
                if (existingDetails.Any())
                {
                    foreach (var detail in existingDetails)
                    {
                        detail.AccountId = entity.AccountId;
                        detail.CurrencyId = entity.CurrencyId;
                        detail.ExchangeRate = exchangeRate;
                        detail.Description = entity.Description;
                        detail.LastModifiedDate = DateTime.Now;

                        if (existingDoc.TypeId == 1)
                        {
                            detail.Debit = total;
                            detail.Credit = 0;
                            detail.Item1QuantityDebit = entity.Item1Quantity ?? 0;
                            detail.Item2QuantityDebit = entity.Item2Quantity ?? 0;
                            detail.Item3QuantityDebit = entity.Item3Quantity ?? 0;
                            detail.Item1QuantityCredit = 0;
                            detail.Item2QuantityCredit = 0;
                            detail.Item3QuantityCredit = 0;
                            detail.LocalDebit = total * exchangeRate;
                            detail.LocalCredit = 0;
                        }
                        else if (existingDoc.TypeId == 2)
                        {
                            detail.Credit = total;
                            detail.Debit = 0;
                            detail.Item1QuantityDebit = 0;
                            detail.Item2QuantityDebit = 0;
                            detail.Item3QuantityDebit = 0;
                            detail.Item1QuantityCredit = entity.Item1Quantity ?? 0;
                            detail.Item2QuantityCredit = entity.Item2Quantity ?? 0;
                            detail.Item3QuantityCredit = entity.Item3Quantity ?? 0;
                            detail.LocalDebit = 0;
                            detail.LocalCredit = total * exchangeRate;
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Result.Success(existingDoc);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Result.Failure<Document>(new Error(ex.Message));
            }
        }

        public override async Task<Result<Document>> DeleteAsync(Guid id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var document = await _context.Documents.FindAsync(id);
                if (document == null)
                {
                    return Result.Failure<Document>(new Error("السند غير موجود"));
                }

                var details = _context.DocumentDetails.Where(d => d.DocumentId == id).ToList();
                if (details.Any())
                {
                    _context.DocumentDetails.RemoveRange(details);
                }

                _context.Documents.Remove(document);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Result.Success(document);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Result.Failure<Document>(new Error(ex.Message));
            }
        }

        /*public override async Task<Result<List<Document>>> GetAllAsync()
        {
            try
            {
                var QueryResult = await _dbSet.Include(s => s.User).Include(a => a.Category).ToListAsync();
                return Result.Success<List<Document>?>(QueryResult);
            }
            catch (Exception ex)
            {
                return Result.Failure<List<Document>>(new Error(ex.Message));
            }
        }*/

        public Result<Document> RefreshSerialNum(DocumentDTO documentDTO)
        {
            if (_dbSet != null)
            {
               var QueryResult= _dbSet.Where(d=>d.OrganizationId==documentDTO.OrganizationId && d.TypeId==documentDTO.TypeId)
                    ?.Max(d=>d.SerialNum);
                if(QueryResult!=null)
                {
                    var Document = new Document { TypeId = documentDTO.TypeId, SerialNum = QueryResult.Value + 1 };
                    return Result.Success(Document);
                }
                else
                {
                    var Document = new Document { TypeId = documentDTO.TypeId, SerialNum = 1 };
                    return Result.Success(Document);
                }
            }
            else
            {
                return Result.Failure<Document>(new Error("لم يتم استرجاع رقم السند"));
            }
        }

        public Result<List<AccountStatementViewModel>> GetAccountStatement(Document dto)
        {
            // First check if matching DocumentDetails exist
            IQueryable<DocumentDetail> detailQuery = _context.DocumentDetails;

            if (dto.AccountId.HasValue && dto.AccountId.Value != Guid.Empty)
            {
                detailQuery = detailQuery.Where(dd => dd.AccountId == dto.AccountId);
            }

            if (dto.OrganizationId.HasValue && dto.OrganizationId.Value != Guid.Empty)
            {
                detailQuery = detailQuery.Where(dd => dd.OrganizationId == dto.OrganizationId);
            }

            if (dto.StartDate.HasValue)
                detailQuery = detailQuery.Where(dd => dd.AddDate >= dto.StartDate);

            if (dto.EndDate.HasValue)
                detailQuery = detailQuery.Where(dd => dd.AddDate <= dto.EndDate);

            if (dto.CurrencyId.HasValue && dto.CurrencyId.Value != Guid.Empty)
                detailQuery = detailQuery.Where(dd => dd.CurrencyId == dto.CurrencyId);

            var list = detailQuery
                .OrderBy(dd => dd.AddDate)
                .Select(dd => new AccountStatementViewModel
                {
                    Date = dd.AddDate,
                    Description = dd.Description,
                    Debit = dd.Debit ?? 0,
                    Credit = dd.Credit ?? 0
                })
                .ToList();

            // Fallback: If no DocumentDetail entries found, query Documents directly
            if (list == null || list.Count == 0)
            {
                IQueryable<Document> docQuery = _context.Documents;

                if (dto.AccountId.HasValue && dto.AccountId.Value != Guid.Empty)
                {
                    docQuery = docQuery.Where(d => d.AccountId == dto.AccountId);
                }

                if (dto.OrganizationId.HasValue && dto.OrganizationId.Value != Guid.Empty)
                {
                    docQuery = docQuery.Where(d => d.OrganizationId == dto.OrganizationId);
                }

                if (dto.StartDate.HasValue)
                    docQuery = docQuery.Where(d => d.AddDate >= dto.StartDate);

                if (dto.EndDate.HasValue)
                    docQuery = docQuery.Where(d => d.AddDate <= dto.EndDate);

                if (dto.CurrencyId.HasValue && dto.CurrencyId.Value != Guid.Empty)
                    docQuery = docQuery.Where(d => d.CurrencyId == dto.CurrencyId);

                list = docQuery
                    .OrderBy(d => d.AddDate)
                    .Select(d => new AccountStatementViewModel
                    {
                        Date = d.AddDate,
                        Description = d.Description,
                        Debit = d.TypeId == 1 ? (d.FinalTotal ?? 0) : 0,
                        Credit = d.TypeId == 2 ? (d.FinalTotal ?? 0) : 0
                    })
                    .ToList();
            }

            // حساب الرصيد التراكمي
            decimal balance = 0;
            foreach (var item in list)
            {
                balance += item.Debit - item.Credit;
                item.Balance = balance;
            }

            return Result.Success(list);
        }
    }

}