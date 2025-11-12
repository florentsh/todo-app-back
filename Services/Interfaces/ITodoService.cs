using BackTodoApi.Dtos;
using BackTodoApi.Helpers;

namespace BackTodoApi.Services.Interfaces
{
    public interface ITodoService
    {
        Task<TodoDto> CreateAsync(CreateTodoDto dto);
        Task<TodoDto> GetByIdAsync(int id);
        Task<IEnumerable<TodoDto>> GetAllAsync();
        Task<PagedResult<TodoDto>> GetPagedAsync(int page, int pageSize);
        Task<PagedResult<TodoDto>> GetPagedByUserIdAsync(string userId, int page, int pageSize);
        Task<TodoDto> UpdateAsync(int id, UpdateTodoDto dto);
        Task<TodoDto> MarkCompleteAsync(int id, bool isCompleted);
        Task DeleteAsync(int id);
        Task AssignTodoAsync(AssignTodoDto dto);
    }
}
