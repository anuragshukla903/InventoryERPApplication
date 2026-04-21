using System.Reflection;
using InventoryERPApp.Interfaces;
using InventoryERPApp.Model;
using Microsoft.EntityFrameworkCore;

namespace InventoryERPApp.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly int _tenantId;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantService tenantService) :
        base(options)
    {
        _tenantId = tenantService?.GetTenantId() ?? 0;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<SubCategory> SubCategories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<PurchaseItem> PurchaseItems { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleItem> SaleItems { get; set; }
    public DbSet<StockLedger> StockLedgers { get; set; }
    public DbSet<StockBatch> StockBatches { get; set; }
    public DbSet<CurrentStock> CurrentStocks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property<bool>(nameof(BaseEntity.IsActive))
                    .HasDefaultValue(true);

                modelBuilder.Entity(entityType.ClrType)
                    .Property<bool>(nameof(BaseEntity.IsDelete))
                    .HasDefaultValue(false);

                var method = typeof(ApplicationDbContext)
                    .GetMethod(nameof(SetGlobalFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(entityType.ClrType);

                method.Invoke(this, new object[] { modelBuilder });
            }
        }
    }

    private void SetGlobalFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : BaseEntity
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => _tenantId == 0 || (e.TenantId == _tenantId && !e.IsDelete));
    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            var now = DateTime.UtcNow;
            if (entry.State == EntityState.Added)
            {
                entry.Entity.TenantId = _tenantId;
                entry.Entity.CreateAt = now;
                entry.Entity.ModifiedAt = now;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.TenantId = _tenantId;
                entry.Entity.ModifiedAt = now;
            }
        }

        return base.SaveChanges();
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.TenantId = _tenantId;
                entry.Entity.CreateAt = now;
                entry.Entity.ModifiedAt = now;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedAt = now;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}