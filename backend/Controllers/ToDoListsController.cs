using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using To_Do_Project.DTOs;
using To_Do_Project.Models;

namespace To_Do_Project.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ToDoListsController : ControllerBase
    {
        private readonly Context _context;

        public ToDoListsController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToDoList>>> GetToDoLists()
        {
            return await _context.ToDoLists
                                 .Include(tdl => tdl.TaskItems)
                                 .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ToDoList>> GetToDoList(int id)
        {
            if (!ToDoListExists(id))
                return NotFound();
            var toDoList = await _context.ToDoLists
                                         .Include(tdl => tdl.TaskItems)
                                         .Include(tdl => tdl.User)
                                         .FirstOrDefaultAsync(tdl => tdl.Id == id);
            if (toDoList == null)
                return NotFound();

            var taskItems = await _context.TaskItems
                                      .Where(ti => ti.ToDoListId == id)
                                      .ToListAsync();
            toDoList.TaskItems = taskItems;

            return toDoList;
        }

        [HttpPost("CreateToDoList/{name}")]
        public async Task<ActionResult<ToDoList>> CreateToDoList(string name)
        {
            // Get current user ID from JWT
            var userIdClaim = User.FindFirst(ClaimTypes.Sid)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User not found in token.");

            if (!int.TryParse(userIdClaim, out int userId))
                return BadRequest("Invalid user ID.");

            // Get user from database
            var user = await _context.Users
                .Include(u => u.ToDoLists) // Make sure ToDoLists navigation property is loaded
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound("User not found.");

            // Create new ToDoList
            var newList = new ToDoList
            {
                Name = name,
                UserId = user.Id // Link to current user
            };

            _context.ToDoLists.Add(newList);
            await _context.SaveChangesAsync();

            return Ok(newList);
        }

        [HttpPut("UpdateToDoList/{id}")]
        public async Task<IActionResult> UpdateToDoList(int id, ToDoList toDoList)
        {
            if (id != toDoList.Id)
                return BadRequest("ID mismatch");

            _context.Entry(toDoList).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToDoListExists(id))
                    return NotFound();
                else
                    throw;
            }

            return Ok("To Do List Updated");
        }
        [HttpPost("AddTaskToToDo/{id}")]
        public async Task<IActionResult> AddTaskToToDo(int id, [FromBody] AddTaskItemDTO taskItemDTO)
        {
            var taskItem = new TaskItem
            {
                Name = taskItemDTO.Name,
                Description = taskItemDTO.Description,
                ToDoListId = taskItemDTO.ToDoListId
            };
            
            if (!ToDoListExists(id))
                return BadRequest("To-Do List with that id doesnt exist.");

            var toDoList = await _context.ToDoLists
                                         .Include(tdl => tdl.TaskItems)
                                         .Include(tdl => tdl.User)
                                         .FirstOrDefaultAsync(tdl => tdl.Id == id);

            if (toDoList == null)
                return NotFound("To-Do List with that id doesnt exist.");

            toDoList.TaskItems.Add(taskItem);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToDoListExists(id))
                    return NotFound();
                else
                    throw;
            }
            return Ok(taskItem);
        }
        [HttpPut("ChangeToDoListName/{id}/{NewName}")]
        public async Task<IActionResult> ChangeToDoListName(int id, string NewName)
        {
            var existingToDoList = await _context.ToDoLists.FindAsync(id);
            if (existingToDoList == null)
                return NotFound();
            existingToDoList.Name = NewName;

            _context.Entry(existingToDoList).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToDoListExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }
        [HttpDelete("DeleteToDoList/{id}")]
        public async Task<IActionResult> DeleteToDoList(int id)
        {
            var toDoList = await _context.ToDoLists
                                         .Include(tdl => tdl.TaskItems)
                                         .FirstOrDefaultAsync(tdl => tdl.Id == id);

            if (toDoList == null)
                return NotFound();

            _context.ToDoLists.Remove(toDoList);
            await _context.SaveChangesAsync();

            return Ok("To Do List was deleted.");
        }

        private bool ToDoListExists(int id)
        {
            return _context.ToDoLists.Any(e => e.Id == id);
        }
    }
}
