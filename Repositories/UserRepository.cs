using BackTodoApi.Dtos;
using Microsoft.AspNetCore.Identity;

namespace BackTodoApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserRepository(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityUser?> GetByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<IdentityUser?> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
        public async Task<IdentityUser?> GetByUsernameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public Task<IEnumerable<IdentityUser>> GetAllAsync()
        {
            return Task.FromResult(_userManager.Users.AsEnumerable());
        }


        public async Task<IdentityResult> CreateUserAsync(UserRegisterDto dto, string password)
        {
            var user = new IdentityUser
            {
                UserName = dto.Username,
                Email = dto.Email
            };

            return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> UpdateAsync(IdentityUser user)
        {
            return await _userManager.UpdateAsync(user);
        }
    }
}
