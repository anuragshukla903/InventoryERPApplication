using InventoryERPApp.DTO.Common;
using InventoryERPApp.DTO.Tenant;
using InventoryERPApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventoryERPApp.Controller;

[Route("api/[controller]")]
[ApiController]
public class TenantController : ControllerBase
{
    private readonly ITenantMasterService _masterService;

    public TenantController(ITenantMasterService tenantMasterService)
    {
        _masterService = tenantMasterService;
    }

    [HttpPost]
    public async Task<ApiResponse<string>> CreateTenant(CreateTenantDto createTenantDto)
    {
       return await _masterService.CreateTenantAsync(createTenantDto);
        
    }
}
