using System.ComponentModel.DataAnnotations;

namespace TodoApp.Api.DTOs
{
    public class TodoItemUpdateDto
    {
        [Required]
        [MaxLength(200)]
        public required string Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public bool IsCompleted { get; set; }
    }
}
