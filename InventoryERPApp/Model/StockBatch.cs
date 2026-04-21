using System.ComponentModel.DataAnnotations;

namespace InventoryERPApp.Model;

public class StockBatch : BaseEntity
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    [MaxLength(50)]
    public string BatchNo { get; set; } = string.Empty;

    [Required]
    public int PurchaseItemId { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal RemainingQty { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Rate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public DateTime CreatedDate { get; set; }
}
