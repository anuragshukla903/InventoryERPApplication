using InventoryERPApp.DTO.Auth;
using InventoryERPApp.Interfaces;
using InventoryERPApp.Model;
using InventoryERPApp.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryERPApp.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IUserService _userService;

        public AuthController(ApplicationDbContext context, IJwtService jwtService, IUserService userService)
        {
            _context = context;
            _jwtService = jwtService;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var tenant = _context.Tenants.FirstOrDefault(x => x.CompanyCode.ToLower() == model.CompanyCode.ToLower() && x.IsActive && !x.IsDelete);
            if (tenant == null)
                return Unauthorized();
            
            var user = _context.Users
                .Include(x => x.Role)
                .FirstOrDefault(x => x.Username == model.Username && x.TenantId==tenant.Id);

            if (user == null)
                return Unauthorized();

            // password verify (hash)
            try
            {
                if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                    return Unauthorized();
            }
            catch (BCrypt.Net.SaltParseException)
            {
                // Handle legacy password hashes (plain text or other formats)
                if (user.PasswordHash != model.Password)
                    return Unauthorized();

                // Update legacy password to BCrypt format
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
                await _context.SaveChangesAsync();
            }

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken(user);

            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return Ok(new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expiry = DateTime.UtcNow.AddMinutes(15)
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(string refreshToken)
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == refreshToken && !x.IsRevoked);

            if (token == null || token.ExpiryDate < DateTime.UtcNow)
                return Unauthorized();

            var user = await _context.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == token.UserId);

            var newAccessToken = _jwtService.GenerateAccessToken(user);

            return Ok(new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = refreshToken,
                Expiry =  DateTime.UtcNow.AddDays(7)
            });
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            await _userService.RegisterAsync(model);
            return Ok();
        }
    }
}