using System.ComponentModel.DataAnnotations;

namespace InventoryERPApp.Model;

public class SaleItem : BaseEntity
{
    [Required]
    public int SaleId { get; set; }
    public Sale Sale { get; set; } = null!;

    [Required]
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Quantity { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Rate { get; set; }
}
