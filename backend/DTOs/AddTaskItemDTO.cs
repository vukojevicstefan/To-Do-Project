namespace To_Do_Project.DTOs
{
    public class AddTaskItemDTO
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int ToDoListId { get; set; }
    }
}