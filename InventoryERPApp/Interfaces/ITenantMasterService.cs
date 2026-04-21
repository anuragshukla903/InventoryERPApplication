using InventoryERPApp.DTO.Common;
using InventoryERPApp.DTO.Tenant;

namespace InventoryERPApp.Interfaces;

public interface ITenantMasterService
{
    Task<ApiResponse<string>> CreateTenantAsync(CreateTenantDto createTenantDto);
}