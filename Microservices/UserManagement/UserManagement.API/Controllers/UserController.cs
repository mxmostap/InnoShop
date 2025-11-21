using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Commands;
using UserManagement.Application.Queries;

namespace UserManagement.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/User/AllUsers
    [HttpGet("AllUsers")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _mediator.Send(new GetAllUsersQuery()));
    }

    // GET: api/User/AllUsers
    [HttpGet]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> GetUserById([FromQuery] int id)
    {
        return Ok(await _mediator.Send(new GetUserByIdQuery {Id = id}));
    }

    // GET: api/User?id=
    [HttpGet("Users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        return Ok(await _mediator.Send(new GetUsersByRoleQuery("User")));
    }

    // PATCH: api/User/Deactivate?id=
    [HttpPatch("Deactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeactivateUser([FromQuery] int id)
    {
        await _mediator.Send(new DeactivateUserCommand { UserId = id });
        return Ok();
    }

    // PUT: api/User/Update
    [HttpPut("Update")]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> UpdateUser(UpdateUserCommand command)
    {
        return Ok(await _mediator.Send(command));
    }
    
    // PUT: api/User/Update/Profile
    [HttpPut("Update/Profile")]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> UpdateUserProfile(UpdateUserProfileCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    // DELETE: api/User/Delete?id=
    [HttpDelete("Delete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser([FromQuery] int id)
    {
        await _mediator.Send(new DeleteUserCommand { UserId = id });
        return Ok();
    }
}