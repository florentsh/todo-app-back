using BackTodoApi.Models;
using Microsoft.AspNetCore.Identity;

namespace BackTodoApi.Services.Interfaces;

public interface ITokenService
{
    Task<(string accessToken, string refreshToken)> GenerateTokensAsync(ApplicationUser user);
    Task<ApplicationUser?> ValidateRefreshTokenAsync(string refreshToken);
    Task RevokeRefreshTokenAsync(string refreshToken);
}
