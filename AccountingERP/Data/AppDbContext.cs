using System;
using Microsoft.EntityFrameworkCore;
using AccountingERP.Models;
using AccountingERP.Services;

namespace AccountingERP.Data
{
    public class AppDbContext : DbContext
    {
        private readonly ITenantService? _tenantService;

        public AppDbContext(DbContextOptions<AppDbContext> options, ITenantService? tenantService = null)
            : base(options)
        {
            _tenantService = tenantService;
        }

        public DbSet<Organization> Organizations { get; set; } = null!;
        public DbSet<Branch> Branches { get; set; } = null!;
        public DbSet<FiscalYear> FiscalYears { get; set; } = null!;
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<CostCenter> CostCenters { get; set; } = null!;
        public DbSet<Document> Documents { get; set; } = null!;
        public DbSet<DocumentDetails> DocumentDetails { get; set; } = null!;
        public DbSet<ApplicationUser> Users { get; set; } = null!;
        public DbSet<ApplicationRole> Roles { get; set; } = null!;
        public DbSet<RolePermission> RolePermissions { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Account self-referencing relationship
            modelBuilder.Entity<Account>()
                .HasOne(a => a.ParentAccount)
                .WithMany(a => a.SubAccounts)
                .HasForeignKey(a => a.ParentAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Document to Details relationship
            modelBuilder.Entity<DocumentDetails>()
                .HasOne(d => d.Document)
                .WithMany(m => m.Details)
                .HasForeignKey(d => d.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DocumentDetails>()
                .HasOne(d => d.Account)
                .WithMany()
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Organization Multi-tenancy Query Filters
            modelBuilder.Entity<Account>().HasQueryFilter(e => _tenantService == null || _tenantService.OrganizationId == Guid.Empty || e.OrganizationId == _tenantService.OrganizationId);
            modelBuilder.Entity<CostCenter>().HasQueryFilter(e => _tenantService == null || _tenantService.OrganizationId == Guid.Empty || e.OrganizationId == _tenantService.OrganizationId);
            modelBuilder.Entity<Document>().HasQueryFilter(e => _tenantService == null || _tenantService.OrganizationId == Guid.Empty || e.OrganizationId == _tenantService.OrganizationId);
            modelBuilder.Entity<Branch>().HasQueryFilter(e => _tenantService == null || _tenantService.OrganizationId == Guid.Empty || e.OrganizationId == _tenantService.OrganizationId);
            modelBuilder.Entity<FiscalYear>().HasQueryFilter(e => _tenantService == null || _tenantService.OrganizationId == Guid.Empty || e.OrganizationId == _tenantService.OrganizationId);
            modelBuilder.Entity<ApplicationUser>().HasQueryFilter(e => _tenantService == null || _tenantService.OrganizationId == Guid.Empty || e.OrganizationId == _tenantService.OrganizationId);
            modelBuilder.Entity<ApplicationRole>().HasQueryFilter(e => _tenantService == null || _tenantService.OrganizationId == Guid.Empty || e.OrganizationId == _tenantService.OrganizationId);
            modelBuilder.Entity<AuditLog>().HasQueryFilter(e => _tenantService == null || _tenantService.OrganizationId == Guid.Empty || e.OrganizationId == _tenantService.OrganizationId);

            // Decimal conversions & precision
            modelBuilder.Entity<Account>().Property(a => a.OpeningBalance).HasConversion<double>();
            modelBuilder.Entity<Document>().Property(d => d.TotalDebit).HasConversion<double>();
            modelBuilder.Entity<Document>().Property(d => d.TotalCredit).HasConversion<double>();
            modelBuilder.Entity<DocumentDetails>().Property(dd => d.Debit).HasConversion<double>();
            modelBuilder.Entity<DocumentDetails>().Property(dd => d.Credit).HasConversion<double>();
        }
    }
}