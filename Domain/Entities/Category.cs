namespace Domain.Entities
{
    public class Category : BaseEntity
    {        
        [MaxLength(50)]
        public string? Name { get; set; }
        
        public DateTime? AddDate { get; set; } = DateTime.Now;
        public virtual List<Account>? Accounts { get; set; }

        public User? User { get; set; }
        public Organization? Organization { get; set; }
    }
}
