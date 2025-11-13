using MediatR;
using UserManagement.Application.Commands;
using UserManagement.Application.DTOs;
using UserManagement.Infrastructure.Persistance;

namespace UserManagement.Application.Handlers;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly EFDBContext _context;
    //private readonly 

    public LoginCommandHandler(
        EFDBContext context)
    {
        _context = context;
    }
}