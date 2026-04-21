namespace InventoryERPApp.Model;

public class Tenant 
{
    public int Id { get; set; }
    public string TenantName { get; set; }
    public string CompanyCode { get; set; }
    public String Address { get; set; }
    public string SubscriptionPlan { get; set; }
    public bool IsActive { get; set; }
    public bool IsDelete { get; set; }
    public bool IsShow { get; set; }
}
