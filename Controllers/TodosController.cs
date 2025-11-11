using BackTodoApi.Dtos;
using BackTodoApi.DTOs;
using BackTodoApi.Exceptions;
using BackTodoApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackTodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TodoController : ControllerBase
{
    private readonly ITodoService _service;

    public TodoController(ITodoService service)
    {
        _service = service;
    }

    // GET api/todo/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            var todo = await _service.GetByIdAsync(id, userId, isAdmin);
            return Ok(todo);
        }
        catch (NotFoundException nf)
        {
            return NotFound(new { message = nf.Message });
        }
        catch (BadRequestException) 
        {
            return Forbid();
        }
        catch
        {
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    // GET api/todo?page=1&pageSize=20
    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            var isAdmin = User.IsInRole("Admin");
            if (isAdmin)
            {
                var paged = await _service.GetPagedAsync(page, pageSize);
                return Ok(paged);
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "Invalid token or user not found." });

                var pagedUser = await _service.GetPagedByUserIdAsync(userId, page, pageSize);
                return Ok(pagedUser);
            }
        }
        catch
        {
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    // POST api/todo
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTodoDto dto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId)) dto.UserId = userId;

            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
        catch (BadRequestException br)
        {
            return BadRequest(new { message = br.Message });
        }
        catch
        {
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    // PUT api/todo/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTodoDto dto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            var updated = await _service.UpdateAsync(id, dto, userId, isAdmin);
            return Ok(updated);
        }
        catch (NotFoundException nf)
        {
            return NotFound(new { message = nf.Message });
        }
        catch (BadRequestException)
        {
            return Forbid();
        }
        catch
        {
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    // PATCH api/todo/{id}/complete 
    [HttpPatch("{id}/complete")]
    public async Task<IActionResult> MarkComplete(int id, [FromBody] MarkCompleteDto body)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            var updated = await _service.MarkCompleteAsync(id, body.IsCompleted, userId, isAdmin);
            return Ok(updated);
        }
        catch (NotFoundException nf)
        {
            return NotFound(new { message = nf.Message });
        }
        catch (BadRequestException)
        {
            return Forbid();
        }
        catch
        {
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    // DELETE api/todo/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            await _service.DeleteAsync(id, userId, isAdmin);
            return NoContent();
        }
        catch (NotFoundException nf)
        {
            return NotFound(new { message = nf.Message });
        }
        catch (BadRequestException)
        {
            return Forbid();
        }
        catch
        {
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
    // POST api/todo/assign
    [HttpPost("assign")]
    [Authorize(Roles = "Admin")] 
    public async Task<IActionResult> Assign([FromBody] AssignTodoDto dto)
    {
        try
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            await _service.AssignTodoAsync(dto, currentUserId, isAdmin);
            return NoContent();
        }
        catch (NotFoundException nf)
        {
            return NotFound(new { message = nf.Message });
        }
        catch (BadRequestException br)
        {
            return BadRequest(new { message = br.Message });
        }
        catch
        {
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

}
