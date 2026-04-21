using InventoryERPApp.DTO.Supplier;

namespace InventoryERPApp.Interfaces;

public interface ISupplierService
{
    Task<SupplierResponse> CreateSupplierAsync(SupplierCreateRequest request);
    Task<SupplierResponse?> GetSupplierByIdAsync(int id);
    Task<List<SupplierResponse>> GetAllSuppliersAsync();
    Task<SupplierResponse?> UpdateSupplierAsync(SupplierUpdateRequest request);
    Task<bool> DeleteSupplierAsync(int id);
    Task<bool> ToggleSupplierStatusAsync(int id);
}
