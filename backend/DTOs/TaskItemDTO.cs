public class TaskItemDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool Checked { get; set; }
}