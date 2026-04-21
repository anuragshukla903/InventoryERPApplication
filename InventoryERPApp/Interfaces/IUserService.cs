using InventoryERPApp.DTO.Auth;

namespace InventoryERPApp.Interfaces;

public interface IUserService
{
    Task RegisterAsync(RegisterDto registerDto);
}