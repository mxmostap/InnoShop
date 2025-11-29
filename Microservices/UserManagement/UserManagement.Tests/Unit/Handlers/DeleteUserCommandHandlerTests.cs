using Moq;
using UserManagement.Application.Commands;
using UserManagement.Application.Exceptions;
using UserManagement.Application.Handlers;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Repositories;

namespace UserManagement.Tests.Unit.Handlers;

public class DeleteUserCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DeleteUserCommandHandler _handler;

    public DeleteUserCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new DeleteUserCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidUserId_DeletesUserAndReturnsUnit()
    {
        var userId = 123;
        var command = new DeleteUserCommand { UserId = userId };
        var user = new User { Id = userId, UserName = "testuser" };

        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsType<MediatR.Unit>(result);
        _unitOfWorkMock.Verify(u => u.Users.GetByIdAsync(userId), Times.Once);
        _unitOfWorkMock.Verify(u => u.Users.Remove(user), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsNotFoundException()
    {
        var userId = 123;
        var command = new DeleteUserCommand { UserId = userId };

        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(userId))
            .ReturnsAsync((User)null);

        var exception =
            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal($"Пользователь с ID \"{userId}\" не найден.", exception.Message);
        _unitOfWorkMock.Verify(u => u.Users.GetByIdAsync(userId), Times.Once);
        _unitOfWorkMock.Verify(u => u.Users.Remove(It.IsAny<User>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsRemoveWithCorrectUser()
    {
        var userId = 12;
        var command = new DeleteUserCommand { UserId = userId };
        var user = new User { Id = userId, UserName = "testuser" };
        User removedUser = null;

        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.Users.Remove(It.IsAny<User>()))
            .Callback<User>(u => removedUser = u);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsType<MediatR.Unit>(result);
        Assert.NotNull(removedUser);
        Assert.Equal(userId, removedUser.Id);
        Assert.Equal("testuser", removedUser.UserName);
    }
}