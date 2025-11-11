using BackTodoApi.Data;
using BackTodoApi.Helpers;
using BackTodoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BackTodoApi.Repositories.Interfaces;

public class TodoRepository : ITodoRepository
{
    private readonly TodoContext _context;

    public TodoRepository(TodoContext context)
    {
        _context = context;
    }

    public async Task<Todo> AddAsync(Todo todo)
    {
        _context.Todos.Add(todo);
        await _context.SaveChangesAsync();
        return todo;
    }

    public async Task DeleteAsync(Todo todo)
    {
        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Todo>> GetAllAsync()
        => await _context.Todos.Include(t => t.User).ToListAsync();

    public async Task<Todo?> GetByIdAsync(int id)
        => await _context.Todos.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id);

    public async Task UpdateAsync(Todo todo)
    {
        _context.Todos.Update(todo);
        await _context.SaveChangesAsync();
    }

    public async Task<PagedResult<Todo>> GetPagedAsync(int page, int pageSize)
    {
        var total = await _context.Todos.CountAsync();
        var items = await _context.Todos.Include(t => t.User)
            .OrderBy(t => t.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Todo>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = total
        };
    }

    public async Task<PagedResult<Todo>> GetPagedByUserIdAsync(string userId, int page, int pageSize)
    {
        var query = _context.Todos.Include(t => t.User)
                                  .Where(t => t.UserId == userId);

        var total = await query.CountAsync();
        var items = await query.OrderBy(t => t.Id)
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();

        return new PagedResult<Todo>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = total
        };
    }
    public async Task AssignTodoToUserAsync(int todoId, string userId)
    {
        var todo = await _context.Todos.FindAsync(todoId);
        if (todo == null) throw new KeyNotFoundException($"Todo {todoId} not found");

        todo.UserId = userId;
        _context.Todos.Update(todo);
        await _context.SaveChangesAsync();
    }

}
