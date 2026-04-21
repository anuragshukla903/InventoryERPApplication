using System.ComponentModel.DataAnnotations;

namespace InventoryERPApp.DTO.Purchase;

public class PurchaseCreateRequest
{
    [Required]
    public int SupplierId { get; set; }

    [Required]
    [MaxLength(50)]
    public string InvoiceNo { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public List<PurchaseItemRequest> Items { get; set; } = new();
}

public class PurchaseItemRequest
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Quantity { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Rate { get; set; }

    [MaxLength(50)]
    public string? BatchNo { get; set; }

    public DateTime? ExpiryDate { get; set; }
}
