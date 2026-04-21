using System.ComponentModel.DataAnnotations;

namespace InventoryERPApp.Model;

public class CurrentStock : BaseEntity
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Quantity { get; set; }
}
