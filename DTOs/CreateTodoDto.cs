namespace BackTodoApi.Dtos;

public class CreateTodoDto
{
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public string? UserId { get; set; }
}
