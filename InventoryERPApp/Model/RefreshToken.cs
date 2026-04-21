namespace InventoryERPApp.Model;

public class RefreshToken : BaseEntity
{
    public int UserId { get; set; }
    public string Token { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsRevoked { get; set; }
    public User User { get; set; } // optional
}
