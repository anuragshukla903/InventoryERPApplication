using System.ComponentModel.DataAnnotations;

namespace InventoryERPApp.Model;

public class Customer : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? GSTIN { get; set; }

    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
