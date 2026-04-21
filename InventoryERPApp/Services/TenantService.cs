using InventoryERPApp.DTO.Common;
using InventoryERPApp.DTO.Tenant;
using InventoryERPApp.Interfaces;
using InventoryERPApp.Persistence;
using InventoryERPApp.Model;
using Microsoft.EntityFrameworkCore;

namespace InventoryERPApp.Services;

public class TenantService : ITenantService
{
    
    private readonly IHttpContextAccessor _httpContextAccessor;
    public TenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

   
    public int GetTenantId()
    {
        var tenantId = _httpContextAccessor.HttpContext?.User?
            .FindFirst("TenantId")?.Value;

        if (string.IsNullOrEmpty(tenantId))
            return 0; // 👈 startup/login safe

        return int.Parse(tenantId);
    }
}
