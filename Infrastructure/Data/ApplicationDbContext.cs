namespace Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseInMemoryDatabase("AsmaFinalProjectInMemoryDB");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure precision and scale for decimal properties
            modelBuilder.Entity<Currency>(entity =>
            {
                entity.Property(e => e.CurrentExchangeRate)
                      .HasPrecision(18, 2); // Example: 18 total digits, 6 decimal places
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.Property(e => e.ExchangeRate).HasPrecision(18, 2);
                entity.Property(e => e.FinalTotal).HasPrecision(18, 2);
                entity.Property(e => e.Item1Price).HasPrecision(18, 2);
                entity.Property(e => e.Item1Quantity).HasPrecision(18, 2);
                entity.Property(e => e.Item2Price).HasPrecision(18, 2);
                entity.Property(e => e.Item2Quantity).HasPrecision(18, 2);
                entity.Property(e => e.Item3Price).HasPrecision(18, 2);
                entity.Property(e => e.Item3Quantity).HasPrecision(18, 2);
            });

            modelBuilder.Entity<DocumentDetail>(entity =>
            {
                entity.Property(e => e.Credit).HasPrecision(18, 2);
                entity.Property(e => e.Debit).HasPrecision(18, 2);
                entity.Property(e => e.ExchangeRate).HasPrecision(18, 2);
                entity.Property(e => e.Item1QuantityCredit).HasPrecision(18, 2);
                entity.Property(e => e.Item1QuantityDebit).HasPrecision(18, 2);
                entity.Property(e => e.Item2QuantityCredit).HasPrecision(18, 2);
                entity.Property(e => e.Item2QuantityDebit).HasPrecision(18, 2);
                entity.Property(e => e.Item3QuantityCredit).HasPrecision(18, 2);
                entity.Property(e => e.Item3QuantityDebit).HasPrecision(18, 2);
                entity.Property(e => e.LocalCredit).HasPrecision(18, 2);
                entity.Property(e => e.LocalDebit).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.Property(e => e.Price).HasPrecision(18, 2);
            });

            base.OnModelCreating(modelBuilder);
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Account>? Accounts { get; set; } = default;
        public DbSet<Category>? Categories { get; set; } = default;

        public DbSet<Currency>? Currencies { get; set; } = default;
        public DbSet<Document>? Documents { get; set; } = default;
        public DbSet<DocumentDetail>? DocumentDetails  { get; set; } = default;
        public DbSet<Item>? Items { get; set; } = default;
        public DbSet<Organization>? Organizations { get; set; } = default;
        public DbSet<Role>? Roles { get; set; } = default;
        public DbSet<User>? Users { get; set; } = default;
        public DbSet<UserRole>? UserRoles { get; set; } = default;
    }
}