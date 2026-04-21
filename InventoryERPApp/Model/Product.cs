using System.ComponentModel.DataAnnotations;

namespace InventoryERPApp.Model;

public class Product : BaseEntity
{
    [Required] [MaxLength(200)] public string ProductName { get; set; } = string.Empty;
    [MaxLength(200)] public string ProductNameHindi { get; set; } = string.Empty;
    [Required] [MaxLength(50)] public string SKU { get; set; } = string.Empty;

    [Required] [MaxLength(20)] public string Unit { get; set; } = string.Empty;

    [Range(0, int.MaxValue)] public int LowStockThreshold { get; set; }

    public bool IsBatchTracked { get; set; }

    public int BrandId { get; set; }

    public int CategoryId { get; set; }
    
    public int SubCategoryId { get; set; }

    [MaxLength(1000)] public string? Description { get; set; }
}