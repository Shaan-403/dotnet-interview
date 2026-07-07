using System.ComponentModel.DataAnnotations;

namespace TodoApi.DTOs;

public class DeleteTodoRequest
{
    [Required]
    public int Id { get; set; }
}