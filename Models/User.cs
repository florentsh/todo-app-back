using Microsoft.AspNetCore.Identity;

namespace BackTodoApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public List<Todo> Todos { get; set; } = new();
    }
}
