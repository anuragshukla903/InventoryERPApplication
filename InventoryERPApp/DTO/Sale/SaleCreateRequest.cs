using System.ComponentModel.DataAnnotations;

namespace InventoryERPApp.DTO.Sale;

public class SaleCreateRequest
{
    public int? CustomerId { get; set; }
    [Required] public DateTime Date { get; set; }
    public string? CustomerName { get; set; }
    public string? Mobile { get; set; }
    [Required] public List<SaleItemRequest> Items { get; set; } = new();
}

public class SaleItemRequest
{
    [Required] public int ProductId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Quantity { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Rate { get; set; }
}