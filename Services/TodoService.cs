using AutoMapper;
using BackTodoApi.Dtos;
using BackTodoApi.Exceptions;
using BackTodoApi.Helpers;
using BackTodoApi.Models;
using BackTodoApi.Repositories.Interfaces;


namespace BackTodoApi.Services.Interfaces;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _repo;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepo;

    public TodoService(ITodoRepository repo, IUserRepository userRepo, IMapper mapper)
    {
        _repo = repo;
        _userRepo = userRepo;
        _mapper = mapper;
    }

    public async Task<TodoDto> CreateAsync(CreateTodoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new BadRequestException("Title is required.");

        var todo = _mapper.Map<Todo>(dto);

        var created = await _repo.AddAsync(todo);
        return _mapper.Map<TodoDto>(created);
    }

    public async Task<TodoDto> GetByIdAsync(int id, string? currentUserId, bool isAdmin)
    {
        var todo = await _repo.GetByIdAsync(id) ?? throw new NotFoundException($"Todo {id} not found");

       
        if (!isAdmin)
        {
            if (!string.IsNullOrEmpty(todo.UserId) && todo.UserId != currentUserId)
                throw new BadRequestException("Access denied");
            if (string.IsNullOrEmpty(todo.UserId))
                throw new BadRequestException("Access denied");
        }

        return _mapper.Map<TodoDto>(todo);
    }

    public async Task<IEnumerable<TodoDto>> GetAllAsync()
    {
        var todos = await _repo.GetAllAsync();
        return todos.Select(t => _mapper.Map<TodoDto>(t));
    }

    public async Task<PagedResult<TodoDto>> GetPagedAsync(int page, int pageSize)
    {
        var paged = await _repo.GetPagedAsync(page, pageSize);
        return new PagedResult<TodoDto>
        {
            Items = paged.Items.Select(t => _mapper.Map<TodoDto>(t)),
            Page = paged.Page,
            PageSize = paged.PageSize,
            TotalItems = paged.TotalItems
        };
    }

    public async Task<PagedResult<TodoDto>> GetPagedByUserIdAsync(string userId, int page, int pageSize)
    {
        var paged = await _repo.GetPagedByUserIdAsync(userId, page, pageSize);
        return new PagedResult<TodoDto>
        {
            Items = paged.Items.Select(t => _mapper.Map<TodoDto>(t)),
            Page = paged.Page,
            PageSize = paged.PageSize,
            TotalItems = paged.TotalItems
        };
    }

    public async Task<TodoDto> UpdateAsync(int id, UpdateTodoDto dto, string? currentUserId, bool isAdmin)
    {
        var todo = await _repo.GetByIdAsync(id) ?? throw new NotFoundException($"Todo {id} not found");

        if (!isAdmin)
        {
            if (!string.IsNullOrEmpty(todo.UserId) && todo.UserId != currentUserId)
                throw new BadRequestException("Access denied");
            if (string.IsNullOrEmpty(todo.UserId))
                throw new BadRequestException("Access denied");
        }

        _mapper.Map(dto, todo);

        await _repo.UpdateAsync(todo);
        return _mapper.Map<TodoDto>(todo);
    }
    public async Task<TodoDto> MarkCompleteAsync(int id, bool isCompleted, string? currentUserId, bool isAdmin)
    {
        var todo = await _repo.GetByIdAsync(id) ?? throw new NotFoundException($"Todo {id} not found");

        if (!isAdmin)
        {
            if (!string.IsNullOrEmpty(todo.UserId) && todo.UserId != currentUserId)
                throw new BadRequestException("Access denied");
            if (string.IsNullOrEmpty(todo.UserId))
                throw new BadRequestException("Access denied");
        }

        todo.IsCompleted = isCompleted;
        await _repo.UpdateAsync(todo);

        return _mapper.Map<TodoDto>(todo);
    }
    public async Task DeleteAsync(int id, string? currentUserId, bool isAdmin)
    {
        var todo = await _repo.GetByIdAsync(id) ?? throw new NotFoundException($"Todo {id} not found");

        if (!isAdmin)
        {
            if (!string.IsNullOrEmpty(todo.UserId) && todo.UserId != currentUserId)
                throw new BadRequestException("Access denied");
            if (string.IsNullOrEmpty(todo.UserId))
                throw new BadRequestException("Access denied");
        }

        await _repo.DeleteAsync(todo);
    }
    public async Task AssignTodoAsync(AssignTodoDto dto, string? currentUserId, bool isAdmin)
    {
        if (dto == null) throw new BadRequestException("Invalid data");

        var todo = await _repo.GetByIdAsync(dto.TodoId) ?? throw new NotFoundException($"Todo {dto.TodoId} not found");

        if (!isAdmin)
            throw new BadRequestException("Only admins can assign todos");

        var user = await _userRepo.GetByIdAsync(dto.UserId);
        if (user == null)
            throw new NotFoundException($"User {dto.UserId} not found");

        await _repo.AssignTodoToUserAsync(dto.TodoId, dto.UserId);
    }

}
