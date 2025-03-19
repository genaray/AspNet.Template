using AspNet.Backend.Feature.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNet.Backend.Feature.AppUser;

/// <summary>
/// The <see cref="UsersController"/> class
/// is a controller that receives and processes user-related requests.
/// This includes all CRUD methods in particular. 
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController(UserService userService) : ControllerBase
{
    
    /// <summary>
    /// Returns all users filtered by a <see cref="searchTerm"/> and pagination;
    /// </summary>
    /// <param name="searchTerm">The term.</param>
    /// <param name="page">The page.</param>
    /// <param name="pageSize">The page-size.</param>
    /// <returns>A <see cref="Task"/> with an <see cref="PaginatedList{T}"/> of <see cref="User"/>s.</returns>
    [HttpGet]
    public async Task<ActionResult<PaginatedList<UserDto>>> GetUsers([FromQuery] string searchTerm = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page <= 0 || pageSize <= 0)
        {
            return BadRequest("Page and PageSize must be greater than zero.");
        }

        var (users, totalCount) = await userService.GetUsersAsync(searchTerm, page, pageSize);
        var result = new PaginatedList<UserDto>(users.ToDtoList(), page, totalCount);

        return Ok(result);
    }
    
    /// <summary>
    /// Returns an <see cref="User"/> by its id.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <returns>A <see cref="Task"/> with the <see cref="User"/>.</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetUser(string id)
    {
        var user = await userService.GetUserByIdAsync(id);

        if (user == null)
        {
            return NotFound(); // 404 Not Found
        }
        
        return Ok(user.ToDto()); // 200 OK
    }
    
    /// <summary>
    /// Updates an <see cref="User"/> by its id.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="user">The <see cref="CreateOrUpdateUserDto"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    [Authorize(Roles = nameof(Roles.ADMIN))]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutUser(string id, CreateOrUpdateUserDto user)
    {
        if (!await userService.UpdateUserAsync(id, user))
        {
            return BadRequest(); // 400 Bad Request or 404 Not Found
        }

        return Ok(); // 200 OK
    }
    
    /// <summary>
    /// Removes an <see cref="User"/> by its id.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    [Authorize(Roles = nameof(Roles.ADMIN))]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        if (!await userService.DeleteUserAsync(id))
        {
            return NotFound(); // 404 Not Found
        }

        return Ok(); // 200 OK
    }
}

