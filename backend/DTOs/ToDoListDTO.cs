public class ToDoListDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<TaskItemDTO> TaskItems { get; set; } = null!;
}