using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace To_Do_Project.Models
{
    public class User
    {
        [Key]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public byte[] Password { get; set; } = Array.Empty<byte>();

        [Required]
        public byte[] Salt { get; set; } = Array.Empty<byte>();

        [Required]
        public string Username { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<ToDoList> ToDoLists { get; set; } = new List<ToDoList>();
    }
}