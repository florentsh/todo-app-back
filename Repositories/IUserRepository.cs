using BackTodoApi.Dtos;
using BackTodoApi.Models;
using Microsoft.AspNetCore.Identity;

namespace BackTodoApi.Repositories
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(string userId);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<ApplicationUser?> GetByUsernameAsync(string username);
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        Task<IdentityResult> CreateUserAsync(UserRegisterDto dto, string password);
        Task<IdentityResult> UpdateAsync(ApplicationUser user);
    }
}
