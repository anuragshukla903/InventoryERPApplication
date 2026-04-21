using System.ComponentModel.DataAnnotations;

namespace InventoryERPApp.Model;

public class Category : BaseEntity
{
    [Required] public string CategoryName { get; set; }
    [Required] public string Code { get; set; }
}