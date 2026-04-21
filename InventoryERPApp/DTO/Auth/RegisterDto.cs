namespace InventoryERPApp.DTO.Auth;

public class RegisterDto
{
    public required string Username { get; set; }
    public string Email { get; set; }
    public required string Password { get; set; }
    public required string Name { get; set; }
    public required string Mobile { get; set; }
    public int RoleId { get; set; }
    public required string CompanyCode { get; set; }
}