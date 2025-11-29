using Moq;
using UserManagement.Application.Commands;
using UserManagement.Application.Common.Interfaces;
using UserManagement.Application.Handlers;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;
using UserManagement.Domain.Repositories;

namespace UserManagement.Tests.Unit.Handlers;

public class ResetPasswordConfirmCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly ResetPasswordConfirmCommandHandler _handler;

    public ResetPasswordConfirmCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tokenServiceMock = new Mock<ITokenService>();
        _handler = new ResetPasswordConfirmCommandHandler(_unitOfWorkMock.Object, _tokenServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccessfulResult()
    {
        var command = new ResetPasswordConfirmCommand 
        { 
            Email = "test@test.com", 
            Token = "valid-token", 
            NewPassword = "newPassword123" 
        };
        var user = new User 
        { 
            Id = 1, 
            Email = "test@test.com", 
            PasswordHash = "old-hash" 
        };

        _unitOfWorkMock.Setup(u => u.Users.GetUserByEmailAsync(command.Email))
                      .ReturnsAsync(user);
        _tokenServiceMock.Setup(t => t.ValidateTokenAsync(user.Id, command.Token, TokenAssignment.PasswordReset))
                        .ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);
        _tokenServiceMock.Setup(t => t.InvalidateTokenAsync(user.Id, command.Token, TokenAssignment.PasswordReset))
                        .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsType<ResetPasswordConfirmCommandResult>(result);
        Assert.True(result.Success);
        Assert.Equal("Пароль успешно изменен", result.Message);
        Assert.True(BCrypt.Net.BCrypt.Verify(command.NewPassword, user.PasswordHash));
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        _tokenServiceMock.Verify(t => t.InvalidateTokenAsync(user.Id, command.Token, TokenAssignment.PasswordReset), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsFailedResult()
    {
        var command = new ResetPasswordConfirmCommand 
        { 
            Email = "nonexistent@test.com", 
            Token = "token", 
            NewPassword = "newPassword123" 
        };

        _unitOfWorkMock.Setup(u => u.Users.GetUserByEmailAsync(command.Email))
                      .ReturnsAsync((User)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsType<ResetPasswordConfirmCommandResult>(result);
        Assert.False(result.Success);
        Assert.Equal("Неверный токен или Email.", result.Message);
        _unitOfWorkMock.Verify(u => u.Users.GetUserByEmailAsync(command.Email), Times.Once);
        _tokenServiceMock.Verify(t => t.ValidateTokenAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<TokenAssignment>()), Times.Never);
    }

    [Fact]
    public async Task Handle_InvalidToken_ReturnsFailedResult()
    {
        var command = new ResetPasswordConfirmCommand 
        { 
            Email = "test@test.com", 
            Token = "invalid-token", 
            NewPassword = "newPassword123" 
        };
        var user = new User { Id = 1, Email = "test@test.com" };

        _unitOfWorkMock.Setup(u => u.Users.GetUserByEmailAsync(command.Email))
                      .ReturnsAsync(user);
        _tokenServiceMock.Setup(t => t.ValidateTokenAsync(user.Id, command.Token, TokenAssignment.PasswordReset))
                        .ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsType<ResetPasswordConfirmCommandResult>(result);
        Assert.False(result.Success);
        Assert.Equal("Неверный или просроченный токен.", result.Message);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        _tokenServiceMock.Verify(t => t.InvalidateTokenAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<TokenAssignment>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidToken_UpdatesPasswordHash()
    {
        var command = new ResetPasswordConfirmCommand 
        { 
            Email = "test@test.com", 
            Token = "valid-token", 
            NewPassword = "newSecurePassword123" 
        };
        var user = new User 
        { 
            Id = 1, 
            Email = "test@test.com", 
            PasswordHash = "old-hash" 
        };
        string newPasswordHash = null;

        _unitOfWorkMock.Setup(u => u.Users.GetUserByEmailAsync(command.Email))
                      .ReturnsAsync(user);
        _tokenServiceMock.Setup(t => t.ValidateTokenAsync(user.Id, command.Token, TokenAssignment.PasswordReset))
                        .ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                      .Callback(() => newPasswordHash = user.PasswordHash)
                      .ReturnsAsync(1);
        _tokenServiceMock.Setup(t => t.InvalidateTokenAsync(user.Id, command.Token, TokenAssignment.PasswordReset))
                        .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(newPasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify(command.NewPassword, newPasswordHash));
    }
}