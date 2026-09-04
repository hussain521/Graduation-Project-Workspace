using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountingERP.Models
{
    public class Organization
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? TaxNumber { get; set; }

        [MaxLength(100)]
        public string? CommercialRegister { get; set; }

        [MaxLength(50)]
        public string Currency { get; set; } = "SAR";

        [MaxLength(20)]
        public string CurrencySymbol { get; set; } = "ر.س";

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual ICollection<Branch> Branches { get; set; } = new List<Branch>();
        public virtual ICollection<FiscalYear> FiscalYears { get; set; } = new List<FiscalYear>();
        public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
        public virtual ICollection<CostCenter> CostCenters { get; set; } = new List<CostCenter>();
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }

    public class Branch
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrganizationId { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual Organization? Organization { get; set; }
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
    }

    public class FiscalYear
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrganizationId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsClosed { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public virtual Organization? Organization { get; set; }
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
    }

    public class Account
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrganizationId { get; set; }

        [Required, MaxLength(50)]
        public string AccountNumber { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string NameAr { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? NameEn { get; set; }

        public AccountType AccountType { get; set; }
        public AccountNature AccountNature { get; set; }

        public int Level { get; set; } = 1;
        public bool IsParent { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public Guid? ParentAccountId { get; set; }
        public virtual Account? ParentAccount { get; set; }
        public virtual ICollection<Account> SubAccounts { get; set; } = new List<Account>();

        [Column(TypeName = "decimal(18,2)")]
        public decimal OpeningBalance { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentBalance { get; set; } = 0;

        [MaxLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual Organization? Organization { get; set; }
        public virtual ICollection<DocumentDetail> DocumentDetails { get; set; } = new List<DocumentDetail>();
    }

    public class CostCenter
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrganizationId { get; set; }

        [Required, MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string NameAr { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? NameEn { get; set; }

        public Guid? ParentCostCenterId { get; set; }
        public virtual CostCenter? ParentCostCenter { get; set; }
        public virtual ICollection<CostCenter> SubCostCenters { get; set; } = new List<CostCenter>();

        public bool IsParent { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public virtual Organization? Organization { get; set; }
        public virtual ICollection<DocumentDetail> DocumentDetails { get; set; } = new List<DocumentDetail>();
    }

    public class Document
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrganizationId { get; set; }
        public Guid BranchId { get; set; }
        public Guid FiscalYearId { get; set; }

        public long DocumentNumber { get; set; }

        [Required, MaxLength(50)]
        public string DocumentCode { get; set; } = string.Empty;

        public DocumentType DocumentType { get; set; }
        public DocumentStatus Status { get; set; } = DocumentStatus.Draft;

        public DateTime DocumentDate { get; set; } = DateTime.Today;

        [MaxLength(100)]
        public string? ReferenceNumber { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalDebit { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCredit { get; set; } = 0;

        public Guid CreatedById { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid? PostedById { get; set; }
        public DateTime? PostedAt { get; set; }

        public virtual Organization? Organization { get; set; }
        public virtual Branch? Branch { get; set; }
        public virtual FiscalYear? FiscalYear { get; set; }
        public virtual ApplicationUser? CreatedBy { get; set; }
        public virtual ApplicationUser? PostedBy { get; set; }

        public virtual ICollection<DocumentDetail> Details { get; set; } = new List<DocumentDetail>();
    }

    public class DocumentDetail
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid DocumentId { get; set; }
        public Guid AccountId { get; set; }
        public Guid? CostCenterId { get; set; }

        public int LineIndex { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Debit { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Credit { get; set; } = 0;

        [MaxLength(500)]
        public string? Note { get; set; }

        public virtual Document? Document { get; set; }
        public virtual Account? Account { get; set; }
        public virtual CostCenter? CostCenter { get; set; }
    }

    public class ApplicationUser
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrganizationId { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? PhoneNumber { get; set; }

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public Guid? RoleId { get; set; }
        public virtual ApplicationRole? Role { get; set; }

        public bool IsAdmin { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual Organization? Organization { get; set; }
    }

    public class ApplicationRole
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrganizationId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Description { get; set; }

        public bool IsSystem { get; set; } = false;

        public virtual Organization? Organization { get; set; }
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }

    public class RolePermission
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RoleId { get; set; }

        [Required, MaxLength(100)]
        public string Permission { get; set; } = string.Empty;

        public virtual ApplicationRole? Role { get; set; }
    }

    public class AuditLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrganizationId { get; set; }
        public Guid? UserId { get; set; }

        [MaxLength(100)]
        public string Action { get; set; } = string.Empty;

        [MaxLength(100)]
        public string EntityName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? EntityId { get; set; }

        public string? Details { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}