using Microsoft.EntityFrameworkCore;
namespace To_Do_Project.Models;

public class Context : DbContext
{
    public Context(DbContextOptions<Context> options) : base(options) { }

    public DbSet<TaskItem> TaskItems { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ToDoList> ToDoLists { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ToDoList>()
            .HasMany(tdl => tdl.TaskItems)
            .WithOne(task => task.ToDoList)
            .HasForeignKey(task => task.ToDoListId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.ToDoLists)
            .WithOne(tdl => tdl.User)
            .HasForeignKey(tdl => tdl.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}