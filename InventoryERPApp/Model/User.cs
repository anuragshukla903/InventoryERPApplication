namespace InventoryERPApp.Model;

public class User : BaseEntity
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Name { get; set; }
    public string Mobile { get; set; }
    public int RoleId { get; set; }
   
    public Role Role { get; set; } // Navigation property
}