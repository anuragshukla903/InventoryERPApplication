using System.ComponentModel.DataAnnotations;

namespace InventoryERPApp.Model;

public class StockLedger : BaseEntity
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [MaxLength(10)]
    public string Type { get; set; } = string.Empty; // IN / OUT

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Quantity { get; set; }

    [Required]
    [MaxLength(20)]
    public string ReferenceType { get; set; } = string.Empty; // Purchase / Sale

    [Required]
    public int ReferenceId { get; set; }

    public int? BatchId { get; set; }
}
