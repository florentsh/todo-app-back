using BackTodoApi.Dtos;
using Microsoft.AspNetCore.Identity;

namespace BackTodoApi.Repositories
{
    public interface IUserRepository
    {
        Task<IdentityUser?> GetByIdAsync(string userId);
        Task<IdentityUser?> GetByEmailAsync(string email);
        Task<IdentityUser?> GetByUsernameAsync(string username);
        Task<IEnumerable<IdentityUser>> GetAllAsync();
        Task<IdentityResult> CreateUserAsync(UserRegisterDto dto, string password);
        Task<IdentityResult> UpdateAsync(IdentityUser user);
    }
}
