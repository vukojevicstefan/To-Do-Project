using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using To_Do_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly Context _context;

    public UsersController(Context context)
    {
        _context = context;
    }
    [Authorize]
    [HttpGet("GetAllUsers")]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }

    [Authorize]
    [HttpGet("GetUser/{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    [Authorize]
    [HttpGet("GetCurrentUserData")]
    public async Task<ActionResult> GetCurrentUserData()
    {
        try
        {
            if (User == null)
            {
                return BadRequest("User is null");
            }
            if (!User.Identity!.IsAuthenticated)
            {
                return BadRequest("No logged-in user. Please log in.");
            }

            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity == null)
            {
                return BadRequest("Error with getting the current user");
            }
            var userClaims = identity.Claims;
            int id = int.Parse(userClaims.FirstOrDefault(u => u.Type == ClaimTypes.Sid)!.Value);

            var user = await _context.Users.Where(u => u.Id == id).Include(u => u.ToDoLists).FirstOrDefaultAsync();

            if (user == null)
                return BadRequest("Error with getting data about current user");

            return Ok(user);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [Authorize]
    [HttpDelete("DeleteAccount")]
    public async Task<IActionResult> DeleteUser()
    {
        try
        {
            if (User == null)
            {
                return BadRequest("User is null");
            }
            if (!User.Identity!.IsAuthenticated)
            {
                return BadRequest("No logged-in user. Please log in.");
            }

            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity == null)
            {
                return BadRequest("Error with getting the current user");
            }
            var userClaims = identity.Claims;
            int id = int.Parse(userClaims.FirstOrDefault(u => u.Type == ClaimTypes.Sid)!.Value);

            var user = await _context.Users.Where(u => u.Id == id).Include(u => u.ToDoLists).FirstOrDefaultAsync();

            if (user == null)
                return BadRequest("Error with getting data about current user");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok($"Deleted user with ID: {id} and its related lists and tasks.");

        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [Authorize]
    [HttpGet("CurrentUserToDoLists")]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetCurrentUserToDoLists()
    {
        List<ToDoListDTO> tdlDTOs = new List<ToDoListDTO>();
        var userIdClaim = User.FindFirst(ClaimTypes.Sid)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized("User not found in token.");

        if (!int.TryParse(userIdClaim, out int userId))
            return BadRequest("Invalid user ID.");

        var user = await _context.Users
            .Include(u => u.ToDoLists)
            .ThenInclude(tdl => tdl.TaskItems)
            .FirstOrDefaultAsync(u => u.Id == userId);
        foreach (ToDoList tdl in user!.ToDoLists)
        {
            ToDoListDTO tdlDTO = new ToDoListDTO
            {
                Id = tdl.Id,
                Name = tdl.Name,
                TaskItems = new List<TaskItemDTO>()
            };
            var taskItems = await _context.TaskItems
                                          .Where(ti => ti.ToDoListId == tdl.Id)
                                          .ToListAsync();
            foreach (TaskItem ti in taskItems)
            {
                TaskItemDTO tiDTO = new TaskItemDTO
                {
                    Id = ti.Id,
                    Name = ti.Name,
                    Description = ti.Description!,
                    Checked = ti.Checked
                };
                tdlDTO.TaskItems.Add(tiDTO);
            }
            tdlDTOs.Add(tdlDTO);
        }
       

        if (user == null)
            return NotFound("User not found.");

        return Ok(tdlDTOs);
    }
}