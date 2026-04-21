using InventoryERPApp.DTO.Common;
using InventoryERPApp.DTO.Tenant;
using InventoryERPApp.Interfaces;
using InventoryERPApp.Model;
using InventoryERPApp.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryERPApp.Services;

public class TenantMasterService : ITenantMasterService
{
    private readonly ApplicationDbContext _context;
    
    public TenantMasterService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<ApiResponse<string>> CreateTenantAsync(CreateTenantDto createTenantDto)
    {
        ApiResponse<string> res = new ApiResponse<string> { Success = true, Message = "" };
        // Check if company code already exists
        var existingTenant = await _context.Tenants
            .FirstOrDefaultAsync(t => t.CompanyCode == createTenantDto.CompanyCode);
        
        if (existingTenant != null)
        {
            return res = new ApiResponse<string> { Success = false, Message = "Company code already exists" };
        }

        // Create new tenant
        var tenant = new Tenant
        {
            TenantName = createTenantDto.TenantName,
            CompanyCode = createTenantDto.CompanyCode,
            Address = createTenantDto.Address,
            SubscriptionPlan = createTenantDto.SubscriptionPlan,
            IsActive = true,
            IsDelete = false,
            IsShow = true
        };

        // Add tenant to database
        await _context.Tenants.AddAsync(tenant);
        await _context.SaveChangesAsync();
        res.Message = "Tenant created successfully";
        res.Success = true;
        return res;
    }
}