using InventoryERPApp.DTO.Common;
using InventoryERPApp.Interfaces;
using InventoryERPApp.Model;
using InventoryERPApp.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryERPApp.Services;

public class MasterService : IMasterService
{
    private readonly ApplicationDbContext _context;
    
    public MasterService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<ApiResponse<string>> CreateBrandAsync(string name, string code)
    {
        ApiResponse<string> res = new ApiResponse<string> { Success = true, Message = "" };
        
        // Check if brand code already exists
        var existingBrand = await _context.Brands
            .FirstOrDefaultAsync(b => b.Code == code);
        
        if (existingBrand != null)
        {
            return new ApiResponse<string> { Success = false, Message = "Brand code already exists" };
        }

        // Create new brand
        var brand = new Brand
        {
            Name = name,
            Code = code
        };

        await _context.Brands.AddAsync(brand);
        await _context.SaveChangesAsync();
        
        return new ApiResponse<string> { Success = true, Message = "Brand created successfully" };
    }
    
    public async Task<ApiResponse<string>> UpdateBrandAsync(int id, string name, string code)
    {
        ApiResponse<string> res = new ApiResponse<string> { Success = true, Message = "" };
        
        var brand = await _context.Brands.FindAsync(id);
        if (brand == null)
        {
            return new ApiResponse<string> { Success = false, Message = "Brand not found" };
        }

        // Check if code is being changed and if it already exists
        if (brand.Code != code)
        {
            var existingBrand = await _context.Brands
                .FirstOrDefaultAsync(b => b.Code == code && b.Id != id);
            
            if (existingBrand != null)
            {
                return new ApiResponse<string> { Success = false, Message = "Brand code already exists" };
            }
        }

        brand.Name = name;
        brand.Code = code;

        _context.Entry(brand).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        
        return new ApiResponse<string> { Success = true, Message = "Brand updated successfully" };
    }
    
    public async Task<ApiResponse<string>> DeleteBrandAsync(int id)
    {
        ApiResponse<string> res = new ApiResponse<string> { Success = true, Message = "" };
        
        var brand = await _context.Brands.FindAsync(id);
        if (brand == null)
        {
            return new ApiResponse<string> { Success = false, Message = "Brand not found" };
        }

        _context.Brands.Remove(brand);
        await _context.SaveChangesAsync();
        
        return new ApiResponse<string> { Success = true, Message = "Brand deleted successfully" };
    }
    
    public async Task<ApiResponse<string>> CreateCategoryAsync(string categoryName, string code)
    {
        ApiResponse<string> res = new ApiResponse<string> { Success = true, Message = "" };
        
        // Check if category code already exists
        var existingCategory = await _context.Categories
            .FirstOrDefaultAsync(c => c.Code == code);
        
        if (existingCategory != null)
        {
            return new ApiResponse<string> { Success = false, Message = "Category code already exists" };
        }

        // Create new category
        var category = new Category
        {
            CategoryName = categoryName,
            Code = code
        };

        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        
        return new ApiResponse<string> { Success = true, Message = "Category created successfully" };
    }
    
    public async Task<ApiResponse<string>> UpdateCategoryAsync(int id, string categoryName, string code)
    {
        ApiResponse<string> res = new ApiResponse<string> { Success = true, Message = "" };
        
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return new ApiResponse<string> { Success = false, Message = "Category not found" };
        }

        // Check if code is being changed and if it already exists
        if (category.Code != code)
        {
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Code == code && c.Id != id);
            
            if (existingCategory != null)
            {
                return new ApiResponse<string> { Success = false, Message = "Category code already exists" };
            }
        }

        category.CategoryName = categoryName;
        category.Code = code;

        _context.Entry(category).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        
        return new ApiResponse<string> { Success = true, Message = "Category updated successfully" };
    }
    
    public async Task<ApiResponse<string>> DeleteCategoryAsync(int id)
    {
        ApiResponse<string> res = new ApiResponse<string> { Success = true, Message = "" };
        
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return new ApiResponse<string> { Success = false, Message = "Category not found" };
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        
        return new ApiResponse<string> { Success = true, Message = "Category deleted successfully" };
    }
    
    public async Task<ApiResponse<string>> CreateProductAsync(string productName, string productNameHindi, string sku, string unit, int lowStockThreshold, bool isBatchTracked, int brandId, int categoryId, int subCategoryId, string description)
    {
        ApiResponse<string> res = new ApiResponse<string> { Success = true, Message = "" };
        
        // Check if SKU already exists
        var existingProduct = await _context.Products
            .FirstOrDefaultAsync(p => p.SKU == sku);
        
        if (existingProduct != null)
        {
            return new ApiResponse<string> { Success = false, Message = "Product SKU already exists" };
        }

        // Create new product
        var product = new Product
        {
            ProductName = productName,
            ProductNameHindi = productNameHindi,
            SKU = sku,
            Unit = unit,
            LowStockThreshold = lowStockThreshold,
            IsBatchTracked = isBatchTracked,
            BrandId = brandId,
            CategoryId = categoryId,
            SubCategoryId = subCategoryId,
            Description = description
        };

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        
        return new ApiResponse<string> { Success = true, Message = "Product created successfully" };
    }
    
    public async Task<ApiResponse<string>> UpdateProductAsync(int id, string productName, string productNameHindi, string sku, string unit, int lowStockThreshold, bool isBatchTracked, int brandId, int categoryId, int subCategoryId, string description)
    {
        ApiResponse<string> res = new ApiResponse<string> { Success = true, Message = "" };
        
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return new ApiResponse<string> { Success = false, Message = "Product not found" };
        }

        // Check if SKU is being changed and if it already exists
        if (product.SKU != sku)
        {
            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.SKU == sku && p.Id != id);
            
            if (existingProduct != null)
            {
                return new ApiResponse<string> { Success = false, Message = "Product SKU already exists" };
            }
        }

        product.ProductName = productName;
        product.ProductNameHindi = productNameHindi;
        product.SKU = sku;
        product.Unit = unit;
        product.LowStockThreshold = lowStockThreshold;
        product.IsBatchTracked = isBatchTracked;
        product.BrandId = brandId;
        product.CategoryId = categoryId;
        product.SubCategoryId = subCategoryId;
        product.Description = description;

        _context.Entry(product).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        
        return new ApiResponse<string> { Success = true, Message = "Product updated successfully" };
    }
    
    public async Task<ApiResponse<string>> DeleteProductAsync(int id)
    {
        ApiResponse<string> res = new ApiResponse<string> { Success = true, Message = "" };
        
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return new ApiResponse<string> { Success = false, Message = "Product not found" };
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        
        return new ApiResponse<string> { Success = true, Message = "Product deleted successfully" };
    }
}
