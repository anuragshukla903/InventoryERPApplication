using System.ComponentModel.DataAnnotations;

namespace InventoryERPApp.Model;

public class PurchaseItem : BaseEntity
{
    [Required]
    public int PurchaseId { get; set; }
    public Purchase Purchase { get; set; } = null!;

    [Required]
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Quantity { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Rate { get; set; }

    [MaxLength(50)]
    public string? BatchNo { get; set; }

    public DateTime? ExpiryDate { get; set; }
}
