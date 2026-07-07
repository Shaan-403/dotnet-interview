using Microsoft.AspNetCore.Mvc;
using TodoApi.DTOs;
using TodoApi.Interfaces;
using TodoApi.Models;

namespace TodoApi.Controllers;

[ApiController]
[Route("api")]
public class TodoController : ControllerBase
{
    private readonly ITodoService _todoService;

    public TodoController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    [HttpPost("createTodo")]
    public IActionResult CreateTodo(CreateTodoRequest request)
    {
        try
        {
            var todo = new Todo
            {
                Title = request.Title,
                Description = request.Description
            };

            var result = _todoService.CreateTodo(todo);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("getTodo")]
    public IActionResult GetTodo(GetTodoRequest request)
    {
        try
        {
            if (request.Id.HasValue)
            {
                var todo = _todoService.GetTodoById(request.Id.Value);

                if (todo == null)
                    return NotFound();

                return Ok(todo);
            }

            return Ok(_todoService.GetAllTodos());
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("updateTodo")]
    public IActionResult UpdateTodo(UpdateTodoRequest request)
    {
        try
        {
            var existing = _todoService.GetTodoById(request.Id);

            if (existing == null)
                return NotFound();

            var todo = new Todo
            {
                Title = request.Title,
                Description = request.Description,
                IsCompleted = request.IsCompleted
            };

            var result = _todoService.UpdateTodo(request.Id, todo);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("deleteTodo")]
    public IActionResult DeleteTodo(DeleteTodoRequest request)
    {
        try
        {
            return _todoService.DeleteTodo(request.Id)
                ? Ok(new { message = "Todo deleted successfully" })
                : NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}