using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace To_Do_Project.Models
{

    public class TaskItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Color { get; set; } = "#000000";

        public bool Checked { get; set; }

        public string? Description { get; set; }

        public TimeSpan? Time { get; set; }

        public int ToDoListId { get; set; }
        [JsonIgnore]
        public ToDoList? ToDoList { get; set; } = null!;
    }
}