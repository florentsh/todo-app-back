using Microsoft.AspNetCore.Identity;

namespace BackTodoApi.Services;

public interface ITokenService
{
    Task<(string accessToken, string refreshToken)> GenerateTokensAsync(IdentityUser user);
    Task<IdentityUser?> ValidateRefreshTokenAsync(string refreshToken);
    Task RevokeRefreshTokenAsync(string refreshToken);
}
