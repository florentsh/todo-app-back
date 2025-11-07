using Microsoft.AspNetCore.Identity;

namespace BackTodoApi.Models;

public class Todo
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public bool IsCompleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? UserId { get; set; }
    public IdentityUser? User { get; set; }
}
