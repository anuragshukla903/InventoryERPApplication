using System.ComponentModel.DataAnnotations;

namespace InventoryERPApp.Model;

public class SubCategory : BaseEntity
{
    [Required] public string SubCategoryName { get; set; } = string.Empty;
    [Required] public string Code { get; set; } = string.Empty;
    
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
