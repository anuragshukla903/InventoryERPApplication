using System;

namespace InventoryERPApp.Model;

public class BaseEntity
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsDelete { get; set; }
    public bool IsShow { get; set; }
}
