using InventoryERPApp.DTO.Common;

namespace InventoryERPApp.Interfaces;

public interface IMasterService
{
    Task<ApiResponse<string>> CreateBrandAsync(string name, string code);
    Task<ApiResponse<string>> UpdateBrandAsync(int id, string name, string code);
    Task<ApiResponse<string>> DeleteBrandAsync(int id);
    
    Task<ApiResponse<string>> CreateCategoryAsync(string categoryName, string code);
    Task<ApiResponse<string>> UpdateCategoryAsync(int id, string categoryName, string code);
    Task<ApiResponse<string>> DeleteCategoryAsync(int id);
    
    Task<ApiResponse<string>> CreateProductAsync(string productName, string productNameHindi, string sku, string unit, int lowStockThreshold, bool isBatchTracked, int brandId, int categoryId, int subCategoryId, string description);
    Task<ApiResponse<string>> UpdateProductAsync(int id, string productName, string productNameHindi, string sku, string unit, int lowStockThreshold, bool isBatchTracked, int brandId, int categoryId, int subCategoryId, string description);
    Task<ApiResponse<string>> DeleteProductAsync(int id);
}
