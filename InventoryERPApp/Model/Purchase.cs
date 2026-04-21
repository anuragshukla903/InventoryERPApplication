using System.ComponentModel.DataAnnotations;

namespace InventoryERPApp.Model;

public class Purchase : BaseEntity
{
    [Required]
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string InvoiceNo { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal TotalAmount { get; set; }

    public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
}
