namespace Domain.Entities
{
    public class Item : BaseEntity
    {        
        [MaxLength(50)]
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public DateTime? AddDate { get; set; } = DateTime.Now;
        public DateTime? LastModifiedDate { get; set; }

        //[ForeignKey(nameof(Currency))]
        public Guid? CurrencyId { get; set; }
        public virtual Currency? Currency { get; set; }

        public User? User { get; set; }
        public Organization? Organization { get; set; }
    }
}
