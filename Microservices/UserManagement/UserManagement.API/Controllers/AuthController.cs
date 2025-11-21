using Microsoft.AspNetCore.Mvc;
using MediatR;
using UserManagement.Application.Commands;

namespace UserManagement.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    //POST: api/Auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    //POST: api/Auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        return Ok(await _mediator.Send(command));
    }
}