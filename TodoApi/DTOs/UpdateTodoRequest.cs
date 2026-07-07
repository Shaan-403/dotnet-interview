using System.ComponentModel.DataAnnotations;

namespace TodoApi.DTOs;

public class UpdateTodoRequest
{
    [Required]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Title { get; set; }

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }
}