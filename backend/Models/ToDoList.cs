using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace To_Do_Project.Models
{
    public class ToDoList
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Color { get; set; } = "#000000";

        public bool Repeat { get; set; }

        public DateTime? Date { get; set; }
        
        [JsonIgnore]
        public ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();

        public int UserId { get; set; }
        
        [JsonIgnore]
        public User User { get; set; } = null!;
    }
}