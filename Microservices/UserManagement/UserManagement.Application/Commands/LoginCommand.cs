using MediatR;
using UserManagement.Application.DTOs;

namespace UserManagement.Application.Commands;

public class LoginCommand : IRequest<AuthResponse>
{
    public string UserName { get; set; }
    public string Password { get; set; }

    public LoginCommand(){ }
    public LoginCommand(string userName, string password)
    {
        UserName = userName;
        Password = password;
    }
}