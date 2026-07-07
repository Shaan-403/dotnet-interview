using System.ComponentModel.DataAnnotations;

namespace TodoApi.DTOs;

public class CreateTodoRequest
{
    [Required]
    [MaxLength(100)]
    public required string Title { get; set; }

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
}