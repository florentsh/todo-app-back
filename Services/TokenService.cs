using BackTodoApi.Data;
using BackTodoApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BackTodoApi.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly TodoContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenService(IConfiguration config, TodoContext context, UserManager<ApplicationUser> userManager)
        {
            _config = config;
            _context = context;
            _userManager = userManager;
        }

        public async Task<(string accessToken, string refreshToken)> GenerateTokensAsync(ApplicationUser user)
        {
            var keyString = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key missing");
            var key = Encoding.UTF8.GetBytes(keyString);
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = (await _userManager.GetClaimsAsync(user)).ToList();

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            if (!string.IsNullOrEmpty(user.UserName))
                claims.Add(new Claim(ClaimTypes.Name, user.UserName));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _config["Jwt:Issuer"] ?? "BackTodoApi",
                Audience = _config["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            var refreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            _context.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            return (accessToken, refreshToken);
        }

        public async Task<ApplicationUser?> ValidateRefreshTokenAsync(string refreshToken)
        {
            var tokenEntry = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken);

            if (tokenEntry == null || tokenEntry.ExpiresAt < DateTime.UtcNow)
                return null;

            var user = await _userManager.FindByIdAsync(tokenEntry.UserId);
            return user;
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var tokenEntry = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken);
            if (tokenEntry != null)
            {
                _context.RefreshTokens.Remove(tokenEntry);
                await _context.SaveChangesAsync();
            }
        }
    }
}
