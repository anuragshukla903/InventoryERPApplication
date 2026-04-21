using InventoryERPApp.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace InventoryERPApp.Persistence;

public class DataSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(ApplicationDbContext context, ILogger<DataSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            await SeedTenantsAsync();
            await SeedRolesAsync();
            await _context.SaveChangesAsync();
            
            // Reset identity columns after seeding to ensure correct sequence
            await ResetIdentityColumnsAsync();
            
            _logger.LogInformation("Database seeded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }
    private async Task SeedTenantsAsync()
    {
        if (!await _context.Tenants.AnyAsync())
        {
            var tenants = new[]
            {
                new Tenant
                {
                    TenantName = "Anav Infotech Pvt. Ltd.",
                    CompanyCode = "ANV",
                    Address = "Indore, MP, 452011",
                    SubscriptionPlan = "Premium",
                    IsActive = true,
                    IsDelete = false,
                    IsShow = true
                },
            };


            await _context.Tenants.AddRangeAsync(tenants);
            _logger.LogInformation("Seeded {Count} tenants", tenants.Length);
        }
    }
    private async Task SeedRolesAsync()
    {
        if (!await _context.Roles.AnyAsync())
        {
            var roles = new[]
            {
                new Role
                {
                    Name = "Admin",
                    Code = "ADMIN",
                    TenantId = 1,
                    CreateAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                    IsActive = true,
                    IsDelete = false,
                    IsShow = true
                },
                new Role
                {
                    Name = "Super Admin",
                    Code = "SUPER ADMIN",
                    TenantId = 1,
                    CreateAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                    IsActive = true,
                    IsDelete = false,
                    IsShow = true
                },
            };

            await _context.Roles.AddRangeAsync(roles);
            _logger.LogInformation("Seeded {Count} roles", roles.Length);
        }
    }
    
    private async Task ResetIdentityColumnsAsync()
    {
        // Reset identity columns to continue from the correct sequence
        await _context.Database.ExecuteSqlRawAsync(@"
            DECLARE @MaxUsersId INT;
            SELECT @MaxUsersId = ISNULL(MAX(Id), 0) FROM Users;
            DBCC CHECKIDENT ('Users', RESEED, @MaxUsersId);
            
            DECLARE @MaxRolesId INT;
            SELECT @MaxRolesId = ISNULL(MAX(Id), 0) FROM Roles;
            DBCC CHECKIDENT ('Roles', RESEED, @MaxRolesId);
            
            DECLARE @MaxTenantsId INT;
            SELECT @MaxTenantsId = ISNULL(MAX(Id), 0) FROM Tenants;
            DBCC CHECKIDENT ('Tenants', RESEED, @MaxTenantsId);
        ");
    }
}