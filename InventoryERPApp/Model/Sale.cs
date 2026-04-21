using System.ComponentModel.DataAnnotations;

namespace InventoryERPApp.Model;

public class Sale : BaseEntity
{
    [Required]
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal TotalAmount { get; set; }

    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}
