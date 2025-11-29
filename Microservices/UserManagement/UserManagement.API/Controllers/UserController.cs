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

    // GET: api/User/all-users
    [HttpGet("all-users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _mediator.Send(new GetAllUsersQuery()));
    }

    // GET: api/User?id=
    [HttpGet]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> GetUserById([FromQuery] int id)
    {
        return Ok(await _mediator.Send(new GetUserByIdQuery {Id = id}));
    }

    // GET: api/User/all-user-role
    [HttpGet("all-user-role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        return Ok(await _mediator.Send(new GetUsersByRoleQuery("User")));
    }

    //POST: api/User/reset-password
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordCommand command)
    {
        await _mediator.Send(command);
        return Ok(new
        {
            success = true,
            message = "Если пользователь с таким Email существует, " +
                      "ему отправлена инструкция по восстановлению пароля."
        });
    }
    
    //POST: api/User/reset-password-confirm
    [HttpPost("reset-password-confirm")]
    public async Task<IActionResult> ResetPasswordConfirm(ResetPasswordConfirmCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.Success)
            return Ok(result);
        
        return BadRequest(result);
    }
    
    //POST: confirm-email
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ResetPasswordConfirm([FromQuery]ConfirmEmailCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.Success)
            return Ok(result);
        
        return BadRequest(result);
    }
    
    // PATCH: api/User/deactivate?id=
    [HttpPatch("deactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeactivateUser([FromQuery] int id)
    {
        await _mediator.Send(new DeactivateUserCommand { UserId = id });
        return Ok();
    }

    // PUT: api/User/update
    [HttpPut("update")]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> UpdateUser(UpdateUserCommand command)
    {
        return Ok(await _mediator.Send(command));
    }
    
    // PUT: api/User/update-profile
    [HttpPut("update-profile")]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> UpdateUserProfile(UpdateUserProfileCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    // DELETE: api/User/delete?id=
    [HttpDelete("delete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser([FromQuery] int id)
    {
        await _mediator.Send(new DeleteUserCommand { UserId = id });
        return Ok();
    }
}