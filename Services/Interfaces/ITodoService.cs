using BackTodoApi.Dtos;
using BackTodoApi.Helpers;

namespace BackTodoApi.Services.Interfaces;

public interface ITodoService
{
    Task<TodoDto> CreateAsync(CreateTodoDto dto);
    Task<TodoDto> GetByIdAsync(int id, string? currentUserId, bool isAdmin);
    Task<IEnumerable<TodoDto>> GetAllAsync();
    Task<PagedResult<TodoDto>> GetPagedAsync(int page, int pageSize);
    Task<PagedResult<TodoDto>> GetPagedByUserIdAsync(string userId, int page, int pageSize);
    
    
    Task<TodoDto> UpdateAsync(int id, UpdateTodoDto dto, string? currentUserId, bool isAdmin);
    Task<TodoDto> MarkCompleteAsync(int id, bool isCompleted, string? currentUserId, bool isAdmin);
    Task DeleteAsync(int id, string? currentUserId, bool isAdmin);
    Task AssignTodoAsync(AssignTodoDto dto, string? currentUserId, bool isAdmin);


}
