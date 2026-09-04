namespace Domain.Entities
{
    public class DocumentDetail : BaseEntity
    {        
        //[ForeignKey(nameof(Account))]
        public Guid? AccountId { get; set; }
        public virtual Account? Account { get; set; }

        //[ForeignKey(nameof(Document))]
        public Guid? DocumentId { get; set; }
        public virtual Document? Document { get; set; }
        public DateTime? AddDate { get; set; } = DateTime.Now;
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public decimal? LocalDebit { get; set; }
        public decimal? LocalCredit { get; set; }
        public decimal? Item1QuantityDebit { get; set; }
        public decimal? Item1QuantityCredit { get; set; }
        public decimal? Item2QuantityDebit { get; set; }
        public decimal? Item2QuantityCredit { get; set; }
        public decimal? Item3QuantityDebit { get; set; }
        public decimal? Item3QuantityCredit { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        [MaxLength(250)]
        public string? Description { get; set; }
        public decimal? ExchangeRate { get; set; }                        

        //[ForeignKey(nameof(Currency))]
        public Guid? CurrencyId { get; set; }
        public virtual Currency? Currency { get; set; }

        public User? User { get; set; }
        public Organization? Organization { get; set; }
    }
}
