using MediatR;
using UserManagement.Application.DTOs;

namespace UserManagement.Application.Commands;

public class LoginCommand : IRequest<AuthResponse>
{
    public string Email { get; }
    public string Password { get; }

    public LoginCommand(string email, string password)
    {
        Email = email;
        Password = password;
    }
}