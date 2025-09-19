using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models
{
    public class TodoItem
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MinLength(1, ErrorMessage = "Title must not be empty")]
        [MaxLength(100, ErrorMessage = "Title too long")]
        public string? Name { get; set; } = string.Empty;

        [MinLength(10, ErrorMessage = "Description must be at least 10 characters")]
        public string? Description { get; set; } = string.Empty;

        public bool IsComplete { get; set; }

        public TaskStatus Status { get; set; } = TaskStatus.Active;
    }

    public enum TaskStatus
    {
        Active,
        Completed,
        Invalid
    }
}