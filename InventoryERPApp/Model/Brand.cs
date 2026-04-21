using System.ComponentModel.DataAnnotations;

namespace InventoryERPApp.Model;

public class Brand : BaseEntity
{
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string Code { get; set; }
}