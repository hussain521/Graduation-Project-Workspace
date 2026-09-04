namespace Domain.Entities
{
    public class Account : BaseEntity
    {
        [MaxLength(50)]
        public string? Name { get; set; }
        public DateTime? AddDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; }
        

        //[ForeignKey(nameof(Category))]
        public Guid? CategoryId { get; set; }
        public virtual Category? Category { get; set; }        

        public virtual List<DocumentDetail>? DocumentDetails { get; set; }

        public User? User { get; set; }

        public Organization? Organization { get; set; }
    }
}