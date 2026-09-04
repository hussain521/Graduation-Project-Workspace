using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class User : BaseEntity
    {
        //[Required]
        [MaxLength(50)]
        public string? UserName { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Email { get; set; }

        [Required]
        [MaxLength(200)]
        public string? Password { get; set; }

        [NotMapped]
        public string? ConfirmPassword { get; set; }
        public DateTime? AddDate { get; set; } = DateTime.Now;
        public virtual List<Category>? Categories { get; set; }
        public virtual List<DocumentDetail>? DocumentDetails { get; set; }
        public virtual List<Document>? Documents { get; set; }
        public virtual List<Item>? Items { get; set; }
                
        public Organization? Organization { get; set; }             
    }
}