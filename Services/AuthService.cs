using BackTodoApi.Dtos;
using BackTodoApi.Repositories;
using BackTodoApi.Models;
using Microsoft.AspNetCore.Identity;

namespace BackTodoApi.Services;

public class AuthService
{
    private readonly IUserRepository _userRepo;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepo, SignInManager<ApplicationUser> signInManager, ITokenService tokenService)
    {
        _userRepo = userRepo;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    public async Task<object> RegisterAsync(UserRegisterDto dto)
    {
        if (await _userRepo.GetByUsernameAsync(dto.Username) != null)
            throw new ApplicationException("Username already exists");

        var result = await _userRepo.CreateUserAsync(dto, dto.Password);
        if (!result.Succeeded)
            throw new ApplicationException(string.Join(", ", result.Errors.Select(e => e.Description)));

        var user = await _userRepo.GetByUsernameAsync(dto.Username);
        var (accessToken, refreshToken) = await _tokenService.GenerateTokensAsync(user!);

        return new
        {
            accessToken,
            refreshToken,
            user = new { id = user!.Id, username = user.UserName }
        };
    }

    public async Task<object> LoginAsync(UserLoginDto dto)
    {
        var user = await _userRepo.GetByUsernameAsync(dto.Username)
                   ?? throw new ApplicationException("Invalid username or password");

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
            throw new ApplicationException("Invalid username or password");

        var (accessToken, refreshToken) = await _tokenService.GenerateTokensAsync(user);
        return new
        {
            accessToken,
            refreshToken,
            user = new { id = user.Id, username = user.UserName }
        };
    }

    public async Task<object> RefreshAsync(string refreshToken)
    {
        var user = await _tokenService.ValidateRefreshTokenAsync(refreshToken)
                   ?? throw new ApplicationException("Invalid or expired refresh token");

        var (accessToken, newRefreshToken) = await _tokenService.GenerateTokensAsync(user);
        await _tokenService.RevokeRefreshTokenAsync(refreshToken);

        return new
        {
            accessToken,
            refreshToken = newRefreshToken,
            user = new { id = user.Id, username = user.UserName }
        };
    }
}
