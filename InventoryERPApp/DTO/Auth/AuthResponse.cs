namespace InventoryERPApp.DTO.Auth;

public class AuthResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime Expiry { get; set; }
}
