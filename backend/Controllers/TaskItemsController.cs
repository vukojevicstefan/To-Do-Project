using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using To_Do_Project.Models;

namespace To_Do_Project.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class TaskItemsController : ControllerBase
    {
        private readonly Context _context;

        public TaskItemsController(Context context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTaskItems()
        {
            return await _context.TaskItems
                                 .Include(t => t.ToDoList)
                                 .ToListAsync();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTaskItem(int id)
        {
            var taskItem = await _context.TaskItems
                                         .Include(t => t.ToDoList)
                                         .FirstOrDefaultAsync(t => t.Id == id);

            if (taskItem == null)
                return NotFound();

            return taskItem;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<TaskItem>> CreateTaskItem([FromBody] TaskItem taskItem)
        {
            if (taskItem.Name.IsNullOrEmpty())
                return BadRequest("Please enter the task name.");

            _context.TaskItems.Add(taskItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTaskItem), new { id = taskItem.Id }, taskItem);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskItem(int id, TaskItem taskItem)
        {
            if (id != taskItem.Id)
                return BadRequest("Task ID mismatch");

            _context.Entry(taskItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskItemExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [Authorize]
        [HttpPut("ToggleTaskItemChecked/{id}")]
        public async Task<IActionResult> ToggleTaskItemChecked(int id)
        {
            var existingTaskItem = await _context.TaskItems.FindAsync(id);
            if (existingTaskItem == null)
                return NotFound();
            existingTaskItem.Checked = !existingTaskItem.Checked;

            _context.Entry(existingTaskItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskItemExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }
        
        [Authorize]
        [HttpPut("ChangeTaskItemName/{id}/{NewName}")]
        public async Task<IActionResult> ChangeTaskItemName(int id, string NewName)
        {
            var existingTaskItem = await _context.TaskItems.FindAsync(id);
            if (existingTaskItem == null)
                return NotFound();
            existingTaskItem.Name = NewName;

            _context.Entry(existingTaskItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskItemExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }
        
        [Authorize]
        [HttpDelete("DeleteTaskItem/{id}")]
        public async Task<IActionResult> DeleteTaskItem(int id)
        {
            var taskItem = await _context.TaskItems.FindAsync(id);
            if (taskItem == null)
                return NotFound();

            _context.TaskItems.Remove(taskItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        private bool TaskItemExists(int id)
        {
            return _context.TaskItems.Any(e => e.Id == id);
        }
    }
}
