namespace BackTodoApi.Dtos;
public class AssignTodoDto
{
    public int TodoId { get; set; }
    public string UserId { get; set; } = string.Empty;
}