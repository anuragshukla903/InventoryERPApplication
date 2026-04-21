using System.ComponentModel.DataAnnotations;

namespace InventoryERPApp.DTO;

// Brand DTOs
public class BrandCreateDto
{
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string Code { get; set; } = string.Empty;
}

public class BrandUpdateDto
{
    public int Id { get; set; }
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string Code { get; set; } = string.Empty;
}

// Category DTOs
public class CategoryCreateDto
{
    [Required] public string CategoryName { get; set; } = string.Empty;
    [Required] public string Code { get; set; } = string.Empty;
}

public class CategoryUpdateDto
{
    public int Id { get; set; }
    [Required] public string CategoryName { get; set; } = string.Empty;
    [Required] public string Code { get; set; } = string.Empty;
}

// Product DTOs
public class ProductCreateDto
{
    [Required] [MaxLength(200)] public string ProductName { get; set; } = string.Empty;
    [MaxLength(200)] public string ProductNameHindi { get; set; } = string.Empty;
    [Required] [MaxLength(50)] public string SKU { get; set; } = string.Empty;
    [Required] [MaxLength(20)] public string Unit { get; set; } = string.Empty;
    [Range(0, int.MaxValue)] public int LowStockThreshold { get; set; }
    public bool IsBatchTracked { get; set; }
    [Required] public int BrandId { get; set; }
    [Required] public int CategoryId { get; set; }
    [Required] public int SubCategoryId { get; set; }
    [MaxLength(1000)] public string? Description { get; set; }
}

public class ProductUpdateDto
{
    public int Id { get; set; }
    [Required] [MaxLength(200)] public string ProductName { get; set; } = string.Empty;
    [MaxLength(200)] public string ProductNameHindi { get; set; } = string.Empty;
    [Required] [MaxLength(50)] public string SKU { get; set; } = string.Empty;
    [Required] [MaxLength(20)] public string Unit { get; set; } = string.Empty;
    [Range(0, int.MaxValue)] public int LowStockThreshold { get; set; }
    public bool IsBatchTracked { get; set; }
    [Required] public int BrandId { get; set; }
    [Required] public int CategoryId { get; set; }
    [Required] public int SubCategoryId { get; set; }
    [MaxLength(1000)] public string? Description { get; set; }
}
