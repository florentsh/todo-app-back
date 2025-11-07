namespace BackTodoApi.Dtos;

public class TodoDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UserId { get; set; }
    public string? Username { get; set; }
}
