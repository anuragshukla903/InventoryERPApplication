using InventoryERPApp.DTO.Sale;

namespace InventoryERPApp.Interfaces;

public interface ISaleService
{
    Task<SaleResponse> CreateSaleAsync(SaleCreateRequest request);
    Task<SaleResponse?> GetSaleByIdAsync(int id);
    Task<List<SaleResponse>> GetAllSalesAsync();
    Task<bool> DeleteSaleAsync(int id);
}
