using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TodoApi.Controllers;
using TodoApi.DTOs;
using TodoApi.Models;
using TodoApi.Services;
using Xunit;

namespace TodoApi.Tests;

public class UnitTest1
{
    private static TodoService CreateService()
    {
        var solutionRoot = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "../../../../"));

        var dbPath = Path.Combine(solutionRoot, "TodoApi", "todos.db");

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:TodoDb"] = $"Data Source={dbPath}"
            })
            .Build();

        return new TodoService(configuration);
    }

    [Fact]
    public void CreateTodo_ShouldGenerateId_AndCreatedAt()
    {
        var service = CreateService();

        var todo = new Todo
        {
            Title = "Create Test",
            Description = "Testing create"
        };

        var result = service.CreateTodo(todo);

        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("Create Test", result.Title);
        Assert.False(result.IsCompleted);
        Assert.NotEqual(default, result.CreatedAt);
    }

    [Fact]
    public void GetTodoById_ShouldReturnTodo_WhenExists()
    {
        var service = CreateService();

        var created = service.CreateTodo(new Todo
        {
            Title = "Lookup",
            Description = "Lookup Test"
        });

        var result = service.GetTodoById(created.Id);

        Assert.NotNull(result);
        Assert.Equal(created.Id, result!.Id);
        Assert.Equal("Lookup", result.Title);
    }

    [Fact]
    public void GetTodoById_ShouldReturnNull_WhenTodoDoesNotExist()
    {
        var service = CreateService();

        var result = service.GetTodoById(999999);

        Assert.Null(result);
    }

    [Fact]
    public void GetAllTodos_ShouldReturnCollection()
    {
        var service = CreateService();

        var todos = service.GetAllTodos();

        Assert.NotNull(todos);
        Assert.True(todos.Count >= 0);
    }

    [Fact]
    public void UpdateTodo_ShouldUpdateExistingTodo()
    {
        var service = CreateService();

        var created = service.CreateTodo(new Todo
        {
            Title = "Original",
            Description = "Original Description"
        });

        var updated = service.UpdateTodo(created.Id, new Todo
        {
            Title = "Updated",
            Description = "Updated Description",
            IsCompleted = true
        });

        Assert.NotNull(updated);
        Assert.Equal("Updated", updated!.Title);
        Assert.True(updated.IsCompleted);
    }

    [Fact]
    public void UpdateTodo_ShouldReturnNull_WhenTodoDoesNotExist()
    {
        var service = CreateService();

        var result = service.UpdateTodo(999999, new Todo
        {
            Title = "Test",
            Description = "Test"
        });

        Assert.Null(result);
    }

    [Fact]
    public void DeleteTodo_ShouldReturnFalse_WhenTodoDoesNotExist()
    {
        var service = CreateService();

        var result = service.DeleteTodo(999999);

        Assert.False(result);
    }

    [Fact]
    public void Controller_CreateTodo_ShouldReturnOkResult()
    {
        var service = CreateService();

        var controller = new TodoController(service);

        var request = new CreateTodoRequest
        {
            Title = "Controller Test",
            Description = "Controller Description"
        };

        var result = controller.CreateTodo(request);

        Assert.IsType<OkObjectResult>(result);
    }
}