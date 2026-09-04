using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Document : BaseEntity
    {
        public int? SerialNum { get; set; }

        //[ForeignKey(nameof(Account))]
        public Guid? AccountId { get; set; }
        public virtual Account? Account { get; set; }

        [MaxLength(20)]
        public int? TypeId { get; set; }
        public decimal? FinalTotal { get; set; }

        [MaxLength(250)]
        public string? Description { get; set; }
        public DateTime? AddDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public decimal? Item1Price { get; set; }
        public decimal? Item2Price { get; set; }
        public decimal? Item3Price { get; set; }
        public decimal? Item1Quantity { get; set; }
        public decimal? Item2Quantity { get; set; }
        public decimal? Item3Quantity { get; set; }        
        //public virtual User? User { get; set; }

        //[ForeignKey(nameof(Currency))]
        public Guid? CurrencyId { get; set; }
        public virtual Currency? Currency { get; set; }
        public decimal? ExchangeRate { get; set; }
        
        public virtual List<DocumentDetail>? DocumentDetails { get; set; }

        public User? User { get; set; }
        public Organization? Organization { get; set; }

        [NotMapped]
        public DateTime? StartDate { get; set; }

        [NotMapped]
        public DateTime? EndDate { get; set; }
    }
}
