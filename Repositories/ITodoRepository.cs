using BackTodoApi.Helpers;
using BackTodoApi.Models;

namespace BackTodoApi.Repositories;

public interface ITodoRepository
{
    Task<Todo> AddAsync(Todo todo);
    Task DeleteAsync(Todo todo);
    Task<Todo?> GetByIdAsync(int id);
    Task<IEnumerable<Todo>> GetAllAsync();
    Task UpdateAsync(Todo todo);
    Task<PagedResult<Todo>> GetPagedAsync(int page, int pageSize);
    Task<PagedResult<Todo>> GetPagedByUserIdAsync(string userId, int page, int pageSize);
}
