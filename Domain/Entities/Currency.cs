namespace Domain.Entities
{
    public class Currency : BaseEntity
    {        
        [MaxLength(50)]
        public string? Name { get; set; }
        public DateTime? AddDate { get; set; } = DateTime.Now;
        public decimal? CurrentExchangeRate { get; set; }

        [MaxLength(20)]
        public string? Symbol { get; set; }
        public bool IsLocal { get; set; } = false;        
        public virtual List<Document>? Documents { get; set; }
        public virtual List<Item>? Items { get; set; }

        public User? User { get; set; }
        public Organization? Organization { get; set; }
    }
}
