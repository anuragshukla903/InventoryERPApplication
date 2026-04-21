using InventoryERPApp.DTO.Purchase;

namespace InventoryERPApp.Interfaces;

public interface IPurchaseService
{
    Task<PurchaseResponse> CreatePurchaseAsync(PurchaseCreateRequest request);
    Task<PurchaseResponse?> GetPurchaseByIdAsync(int id);
    Task<List<PurchaseResponse>> GetAllPurchasesAsync();
    Task<bool> DeletePurchaseAsync(int id);
}
