using InventoryERPApp.DTO.Auth;
using InventoryERPApp.Interfaces;
using InventoryERPApp.Persistence;
using InventoryERPApp.Model;
using Microsoft.EntityFrameworkCore;

namespace InventoryERPApp.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task RegisterAsync(RegisterDto registerDto)
    {
        // Validate tenant exists
        var tenant = await _context.Tenants
            .FirstOrDefaultAsync(t => t.CompanyCode == registerDto.CompanyCode && t.IsActive);
        
        if (tenant == null)
        {
            throw new Exception("Invalid or inactive company code");
        }

        // Check if user already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == registerDto.Username || u.Email == registerDto.Email);
        
        if (existingUser != null)
        {
            throw new Exception("Username or email already exists");
        }

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        // Create new user
        var user = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = passwordHash,
            Name = registerDto.Name,
            Mobile = registerDto.Mobile,
            RoleId = registerDto.RoleId,
            TenantId =  tenant.Id,
            CreateAt =  DateTime.UtcNow,
        };

        // Add user to database
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}