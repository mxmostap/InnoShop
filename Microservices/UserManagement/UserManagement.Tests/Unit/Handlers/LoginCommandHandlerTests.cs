using Moq;
using UserManagement.Application.Commands;
using UserManagement.Application.Common.Interfaces;
using UserManagement.Application.DTOs;
using UserManagement.Application.Exceptions;
using UserManagement.Application.Handlers;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;
using UserManagement.Domain.Repositories;

namespace UserManagement.Tests.Unit.Handlers;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _jwtServiceMock = new Mock<IJwtService>();
        _handler = new LoginCommandHandler(_unitOfWorkMock.Object, _jwtServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsAuthResponse()
    {
        var command = new LoginCommand("testuser", "password123");
        var user = new User
        {
            Id = 1,
            UserName = "testuser",
            Email = "test@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            IsActive = true,
            Role = UserRole.User,
            Profile = new Profile { FirstName = "John", LastName = "Doe" }
        };
        var expectedToken = "jwt-token-here";

        _unitOfWorkMock.Setup(u => u.Users.GetUserByUsernameAsync(command.UserName))
            .ReturnsAsync(user);
        _jwtServiceMock.Setup(j => j.GenerateToken(user))
            .Returns(expectedToken);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsType<AuthResponse>(result);
        Assert.Equal(expectedToken, result.JwtToken);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.UserName, result.UserName);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.Profile.FirstName, result.FirstName);
        Assert.Equal(user.Profile.LastName, result.LastName);
        Assert.Equal(user.Role, result.Role);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsUnauthorizedAccessException()
    {
        var command = new LoginCommand("nonexistent", "password123");
        _unitOfWorkMock.Setup(u => u.Users.GetUserByUsernameAsync(command.UserName))
            .ReturnsAsync((User)null);

        var exception =
            await Assert.ThrowsAsync<UnauthorizedException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal("Неверные учетные данные.", exception.Message);
    }

    [Fact]
    public async Task Handle_UserInactive_ThrowsUnauthorizedAccessException()
    {
        var command = new LoginCommand("inactiveuser", "password123");
        var user = new User
        {
            Id = 1,
            UserName = "inactiveuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            IsActive = false
        };

        _unitOfWorkMock.Setup(u => u.Users.GetUserByUsernameAsync(command.UserName))
            .ReturnsAsync(user);

        var exception =
            await Assert.ThrowsAsync<UnauthorizedException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal("Неверные учетные данные.", exception.Message);
    }

    [Fact]
    public async Task Handle_InvalidPassword_ThrowsUnauthorizedAccessException()
    {
        var command = new LoginCommand("testuser", "wrongpassword");
        var user = new User
        {
            Id = 1,
            UserName = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
            IsActive = true
        };

        _unitOfWorkMock.Setup(u => u.Users.GetUserByUsernameAsync(command.UserName))
            .ReturnsAsync(user);

        var exception =
            await Assert.ThrowsAsync<UnauthorizedException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal("Неверный пароль.", exception.Message);
    }
}