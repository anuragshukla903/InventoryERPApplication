using InventoryERPApp.Model;

namespace InventoryERPApp.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken(User user);
}