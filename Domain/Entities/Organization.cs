using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Organization : BaseEntity
    {        
        [MaxLength(50)]
        public string? Name { get; set; }

        [MaxLength(250)]
        public string? Emails { get; set; }
        public DateTime? AddDate { get; set; } = DateTime.Now;

        [MaxLength(100)]
        public string? LblRightLine1 { get; set; }

        [MaxLength(100)]
        public string? LblRightLine2 { get; set; }

        [MaxLength(100)]
        public string? LblRightLine3 { get; set; }

        [MaxLength(100)]
        public string? LblRightLine4 { get; set; }

        [MaxLength(250)]
        public string? LogoPath { get; set; }

        [MaxLength(100)]
        public string? LblLeftLine1 { get; set; }

        [MaxLength(100)]
        public string? LblLeftLine2 { get; set; }

        [MaxLength(100)]
        public string? LblLeftLine3 { get; set; }

        [MaxLength(100)]
        public string? LblLeftLine4 { get; set; }
        public bool? IsActive { get; set; }
        public int HeaderTypled { get; set; }

        [NotMapped]
        public override Guid? UserId { get; set; }

        [NotMapped]
        public override Guid? OrganizationId { get; set; }

        public virtual List<User>? Users { get; set; }
    }
}
