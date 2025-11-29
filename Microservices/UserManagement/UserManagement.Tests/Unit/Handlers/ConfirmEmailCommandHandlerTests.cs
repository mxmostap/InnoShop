using Moq;
using UserManagement.Application.Commands;
using UserManagement.Application.Common.Interfaces;
using UserManagement.Application.Handlers;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;
using UserManagement.Domain.Repositories;

namespace UserManagement.Tests.Unit.Handlers;

public class ConfirmEmailCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly ConfirmEmailCommandHandler _handler;

    public ConfirmEmailCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tokenServiceMock = new Mock<ITokenService>();
        _handler = new ConfirmEmailCommandHandler(_unitOfWorkMock.Object, _tokenServiceMock.Object);
    }
    
    [Fact]
    public async Task Handle_UserNotFound_ReturnsFailedResult()
    {
        var command = new ConfirmEmailCommand { Email = "notexisted@test.com", Token = "token" };
        _unitOfWorkMock.Setup(u => u.Users.GetUserByEmailAsync(command.Email))
                      .ReturnsAsync((User)null);

        var result = await _handler.Handle(command, CancellationToken.None);

         Assert.IsType<ConfirmEmailCommandResult>(result);
         Assert.False(result.Success);
         Assert.Equal("Пользователь не найден.", result.Message);
    }

    [Fact]
    public async Task Handle_EmailAlreadyConfirmed_ReturnsFailedResult()
    {
        var command = new ConfirmEmailCommand { Email = "test@test.com", Token = "token" };
        var user = new User { Id = 1, Email = "test@test.com", EmailConfirmed = true };
        _unitOfWorkMock.Setup(u => u.Users.GetUserByEmailAsync(command.Email))
                      .ReturnsAsync(user);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsType<ConfirmEmailCommandResult>(result);
        Assert.False(result.Success);
        Assert.Equal("Email уже подтвержден.", result.Message);
    }

    [Fact]
    public async Task Handle_InvalidToken_ReturnsFailedResult()
    {
        var command = new ConfirmEmailCommand { Email = "test@test.com", Token = "invalid-token" };
        var user = new User { Id = 1, Email = "test@test.com", EmailConfirmed = false };
        _unitOfWorkMock.Setup(u => u.Users.GetUserByEmailAsync(command.Email))
                      .ReturnsAsync(user);
        _tokenServiceMock.Setup(t => t.ValidateTokenAsync(user.Id, command.Token, TokenAssignment.EmailConfirmation))
                        .ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsType<ConfirmEmailCommandResult>(result);
        Assert.False(result.Success);
        Assert.Equal("Неверный или просроченный токен", result.Message);
    }

    [Fact]
    public async Task Handle_ValidToken_ConfirmsEmailAndReturnsSuccess()
    {
        var command = new ConfirmEmailCommand { Email = "test@test.com", Token = "valid-token" };
        var user = new User { Id = 1, Email = "test@test.com", EmailConfirmed = false };
        _unitOfWorkMock.Setup(u => u.Users.GetUserByEmailAsync(command.Email))
                      .ReturnsAsync(user);
        _tokenServiceMock.Setup(t => t.ValidateTokenAsync(user.Id, command.Token, TokenAssignment.EmailConfirmation))
                        .ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.Users.UpdateEmailConfirmedAsync(user.Id, true))
                      .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                      .ReturnsAsync(1);
        _tokenServiceMock.Setup(t => t.InvalidateTokenAsync(user.Id, command.Token, TokenAssignment.EmailConfirmation))
                        .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsType<ConfirmEmailCommandResult>(result);
        Assert.True(result.Success);
        Assert.Equal("Email успешно подтвержден", result.Message);
    }
}