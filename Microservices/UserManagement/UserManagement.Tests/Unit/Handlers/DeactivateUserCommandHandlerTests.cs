using Moq;
using UserManagement.Application.Commands;
using UserManagement.Application.Exceptions;
using UserManagement.Application.Handlers;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Repositories;

namespace UserManagement.Tests.Unit.Handlers;

public class DeactivateUserCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DeactivateUserCommandHandler _handler;

    public DeactivateUserCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new DeactivateUserCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidUserId_DeactivatesUserAndReturnsUnit()
    {
        var userId = 11;
        var command = new DeactivateUserCommand { UserId = userId };
        var user = new User { Id = userId, UserName = "testuser", IsActive = true };

        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsType<MediatR.Unit>(result);
        Assert.False(user.IsActive);
        _unitOfWorkMock.Verify(u => u.Users.GetByIdAsync(userId), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsNotFoundException()
    {
        var userId = 123;
        var command = new DeactivateUserCommand { UserId = userId };

        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(userId))
            .ReturnsAsync((User)null);

        var exception =
            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal($"Пользователь с ID \"{userId}\" не найден.", exception.Message);
        _unitOfWorkMock.Verify(u => u.Users.GetByIdAsync(userId), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ActivatesInactiveUser_ChangesIsActiveToTrue()
    {
        var userId = 12;
        var command = new DeactivateUserCommand { UserId = userId };
        var user = new User { Id = userId, UserName = "testuser", IsActive = false };

        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsType<MediatR.Unit>(result);
        Assert.True(user.IsActive);
        _unitOfWorkMock.Verify(u => u.Users.GetByIdAsync(userId), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}