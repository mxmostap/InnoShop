using Moq;
using UserManagement.Application.Commands;
using UserManagement.Application.Common.Interfaces;
using UserManagement.Application.Exceptions;
using UserManagement.Application.Handlers;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;
using UserManagement.Domain.Repositories;

namespace UserManagement.Tests.Unit.Handlers;

public class ResetPasswordCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly ResetPasswordCommandHandler _handler;

    public ResetPasswordCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tokenServiceMock = new Mock<ITokenService>();
        _emailServiceMock = new Mock<IEmailService>();
        _handler = new ResetPasswordCommandHandler(_unitOfWorkMock.Object, _tokenServiceMock.Object, _emailServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidEmail_GeneratesTokenAndSendsEmail()
    {
        var command = new ResetPasswordCommand { Email = "test@test.com" };
        var user = new User { Id = 1, Email = "test@test.com", UserName = "testuser" };
        var resetToken = "reset-token-123";

        _unitOfWorkMock.Setup(u => u.Users.GetUserByEmailAsync(command.Email))
                      .ReturnsAsync(user);
        _tokenServiceMock.Setup(t => t.GenerateAndSaveTokenAsync(
            user.Id, TokenAssignment.PasswordReset, TimeSpan.FromHours(24)))
                        .ReturnsAsync(resetToken);
        _emailServiceMock.Setup(e => e.SendPasswordResetEmailAsync(user, resetToken))
                        .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsType<MediatR.Unit>(result);
        _unitOfWorkMock.Verify(u => u.Users.GetUserByEmailAsync(command.Email), Times.Once);
        _tokenServiceMock.Verify(t => t.GenerateAndSaveTokenAsync(
            user.Id, TokenAssignment.PasswordReset, TimeSpan.FromHours(24)), Times.Once);
        _emailServiceMock.Verify(e => e.SendPasswordResetEmailAsync(user, resetToken), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsNotFoundException()
    {
        var command = new ResetPasswordCommand { Email = "nonexistent@test.com" };
        _unitOfWorkMock.Setup(u => u.Users.GetUserByEmailAsync(command.Email))
                      .ReturnsAsync((User)null);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal("Ошибка получения запроса.", exception.Message);
        _unitOfWorkMock.Verify(u => u.Users.GetUserByEmailAsync(command.Email), Times.Once);
        _tokenServiceMock.Verify(t => t.GenerateAndSaveTokenAsync(It.IsAny<int>(), It.IsAny<TokenAssignment>(), It.IsAny<TimeSpan>()), Times.Never);
        _emailServiceMock.Verify(e => e.SendPasswordResetEmailAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CallsServicesWithCorrectParameters()
    {
        var command = new ResetPasswordCommand { Email = "user@test.com" };
        var user = new User { Id = 5, Email = "user@test.com", UserName = "user" };
        var resetToken = "generated-token";
        int? actualUserId = null;
        TokenAssignment? actualTokenAssignment = null;
        User actualEmailUser = null;
        string actualEmailToken = null;

        _unitOfWorkMock.Setup(u => u.Users.GetUserByEmailAsync(command.Email))
                      .ReturnsAsync(user);
        _tokenServiceMock.Setup(t => t.GenerateAndSaveTokenAsync(It.IsAny<int>(), It.IsAny<TokenAssignment>(), It.IsAny<TimeSpan>()))
                        .Callback<int, TokenAssignment, TimeSpan>((id, assignment, _) => 
                        {
                            actualUserId = id;
                            actualTokenAssignment = assignment;
                        })
                        .ReturnsAsync(resetToken);
        _emailServiceMock.Setup(e => e.SendPasswordResetEmailAsync(It.IsAny<User>(), It.IsAny<string>()))
                        .Callback<User, string>((u, token) => 
                        {
                            actualEmailUser = u;
                            actualEmailToken = token;
                        })
                        .Returns(Task.CompletedTask);

        await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(user.Id, actualUserId);
        Assert.Equal(TokenAssignment.PasswordReset, actualTokenAssignment);
        Assert.Equal(user, actualEmailUser);
        Assert.Equal(resetToken, actualEmailToken);
    }
}