namespace InventoryERPApp.DTO.Tenant;

public class CreateTenantDto
{
    public string TenantName { get; set; }
    public string CompanyCode { get; set; }
    public string Address { get; set; }
    public string SubscriptionPlan { get; set; }
}
