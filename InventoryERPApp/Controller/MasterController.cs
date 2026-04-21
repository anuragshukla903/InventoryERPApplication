using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryERPApp.Model;
using InventoryERPApp.Persistence;
using InventoryERPApp.DTO;
using InventoryERPApp.DTO.Common;
using InventoryERPApp.Interfaces;

namespace InventoryERPApp.Controller;

[ApiController]
[Route("api/[controller]")]
public class MasterController : ControllerBase
{
    private readonly IMasterService _masterService;
    private readonly ApplicationDbContext _context;

    public MasterController(IMasterService masterService, ApplicationDbContext context)
    {
        _masterService = masterService;
        _context = context;
    }

    // Brand CRUD Operations
    
    [HttpGet("brands")]
    public async Task<ApiResponse<string>> GetBrands()
    {
        var brands = await _context.Brands.ToListAsync();
        return new ApiResponse<string> { Success = true, Message = "Brands retrieved successfully", Data = System.Text.Json.JsonSerializer.Serialize(brands) };
    }

    [HttpGet("brands/{id}")]
    public async Task<ApiResponse<string>> GetBrand(int id)
    {
        var brand = await _context.Brands.FindAsync(id);
        if (brand == null)
        {
            return new ApiResponse<string> { Success = false, Message = "Brand not found" };
        }
        return new ApiResponse<string> { Success = true, Message = "Brand retrieved successfully", Data = System.Text.Json.JsonSerializer.Serialize(brand) };
    }

    [HttpPost("brands")]
    public async Task<ApiResponse<string>> CreateBrand(BrandCreateDto brandDto)
    {
        return await _masterService.CreateBrandAsync(brandDto.Name, brandDto.Code);
    }

    [HttpPut("brands/{id}")]
    public async Task<ApiResponse<string>> UpdateBrand(int id, BrandUpdateDto brandDto)
    {
        return await _masterService.UpdateBrandAsync(id, brandDto.Name, brandDto.Code);
    }

    [HttpDelete("brands/{id}")]
    public async Task<ApiResponse<string>> DeleteBrand(int id)
    {
        return await _masterService.DeleteBrandAsync(id);
    }

    // Category CRUD Operations
    
    [HttpGet("categories")]
    public async Task<ApiResponse<string>> GetCategories()
    {
        var categories = await _context.Categories.ToListAsync();
        return new ApiResponse<string> { Success = true, Message = "Categories retrieved successfully", Data = System.Text.Json.JsonSerializer.Serialize(categories) };
    }

    [HttpGet("categories/{id}")]
    public async Task<ApiResponse<string>> GetCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return new ApiResponse<string> { Success = false, Message = "Category not found" };
        }
        return new ApiResponse<string> { Success = true, Message = "Category retrieved successfully", Data = System.Text.Json.JsonSerializer.Serialize(category) };
    }

    [HttpPost("categories")]
    public async Task<ApiResponse<string>> CreateCategory(CategoryCreateDto categoryDto)
    {
        return await _masterService.CreateCategoryAsync(categoryDto.CategoryName, categoryDto.Code);
    }

    [HttpPut("categories/{id}")]
    public async Task<ApiResponse<string>> UpdateCategory(int id, CategoryUpdateDto categoryDto)
    {
        return await _masterService.UpdateCategoryAsync(id, categoryDto.CategoryName, categoryDto.Code);
    }

    [HttpDelete("categories/{id}")]
    public async Task<ApiResponse<string>> DeleteCategory(int id)
    {
        return await _masterService.DeleteCategoryAsync(id);
    }

    private bool BrandExists(int id)
    {
        return _context.Brands.Any(e => e.Id == id);
    }

    // Product CRUD Operations
    
    [HttpGet("products")]
    public async Task<ApiResponse<string>> GetProducts()
    {
        var products = await _context.Products.ToListAsync();
        return new ApiResponse<string> { Success = true, Message = "Products retrieved successfully", Data = System.Text.Json.JsonSerializer.Serialize(products) };
    }

    [HttpGet("products/{id}")]
    public async Task<ApiResponse<string>> GetProduct(int id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id);
            
        if (product == null)
        {
            return new ApiResponse<string> { Success = false, Message = "Product not found" };
        }
        return new ApiResponse<string> { Success = true, Message = "Product retrieved successfully", Data = System.Text.Json.JsonSerializer.Serialize(product) };
    }

    [HttpPost("products")]
    public async Task<ApiResponse<string>> CreateProduct(ProductCreateDto productDto)
    {
        return await _masterService.CreateProductAsync(
            productDto.ProductName,
            productDto.ProductNameHindi,
            productDto.SKU,
            productDto.Unit,
            productDto.LowStockThreshold,
            productDto.IsBatchTracked,
            productDto.BrandId,
            productDto.CategoryId,
            productDto.SubCategoryId,
            productDto.Description
        );
    }

    [HttpPut("products/{id}")]
    public async Task<ApiResponse<string>> UpdateProduct(int id, ProductUpdateDto productDto)
    {
        return await _masterService.UpdateProductAsync(
            id,
            productDto.ProductName,
            productDto.ProductNameHindi,
            productDto.SKU,
            productDto.Unit,
            productDto.LowStockThreshold,
            productDto.IsBatchTracked,
            productDto.BrandId,
            productDto.CategoryId,
            productDto.SubCategoryId,
            productDto.Description
        );
    }

    [HttpDelete("products/{id}")]
    public async Task<ApiResponse<string>> DeleteProduct(int id)
    {
        return await _masterService.DeleteProductAsync(id);
    }
    private bool CategoryExists(int id)
    {
        return _context.Categories.Any(e => e.Id == id);
    }
    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.Id == id);
    }
}
