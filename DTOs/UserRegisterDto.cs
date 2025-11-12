using System.ComponentModel.DataAnnotations;

namespace BackTodoApi.Dtos;

public class UserRegisterDto
{
    public string Username { get; set; } = "";
    //[Required]
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}
