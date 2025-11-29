using Moq;
using UserManagement.Application.Commands;
using UserManagement.Application.Exceptions;
using UserManagement.Application.Handlers;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Repositories;

namespace UserManagement.Tests.Unit.Handlers;

public class UpdateUserCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdateUserCommandHandler _handler;

    public UpdateUserCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new UpdateUserCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsUpdatedUser()
    {
        var command = new UpdateUserCommand 
        { 
            UserId = 1, 
            UserName = "newusername", 
            Email = "new@test.com", 
            Password = "newpassword123" 
        };
        var user = new User 
        { 
            Id = 1, 
            UserName = "oldusername", 
            Email = "old@test.com", 
            PasswordHash = "old-hash" 
        };

        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(command.UserId))
                      .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                      .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsType<User>(result);
        Assert.Equal(command.UserName, result.UserName);
        Assert.Equal(command.Email, result.Email);
        Assert.True(BCrypt.Net.BCrypt.Verify(command.Password, result.PasswordHash));
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsNotFoundException()
    {
        var command = new UpdateUserCommand 
        { 
            UserId = 123, 
            UserName = "newusername", 
            Email = "new@test.com", 
            Password = "newpassword123" 
        };

        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(command.UserId))
                      .ReturnsAsync((User)null);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));
        Assert.Equal($"Пользователь с ID \"{command.UserId}\" не найден.", exception.Message);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_UpdatesUserPropertiesCorrectly()
    {
        var command = new UpdateUserCommand 
        { 
            UserId = 1, 
            UserName = "updateduser", 
            Email = "updated@test.com", 
            Password = "updatedpassword" 
        };
        var user = new User 
        { 
            Id = 1, 
            UserName = "originaluser", 
            Email = "original@test.com", 
            PasswordHash = "original-hash" 
        };

        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(command.UserId))
                      .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                      .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(command.UserName, user.UserName);
        Assert.Equal(command.Email, user.Email);
        Assert.True(BCrypt.Net.BCrypt.Verify(command.Password, user.PasswordHash));
        Assert.Same(user, result);
    }
}