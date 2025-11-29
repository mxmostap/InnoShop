using Moq;
using UserManagement.Application.Commands;
using UserManagement.Application.Common.Interfaces;
using UserManagement.Application.Exceptions;
using UserManagement.Application.Handlers;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;
using UserManagement.Domain.Repositories;

namespace UserManagement.Tests.Unit.Handlers;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tokenServiceMock = new Mock<ITokenService>();
        _emailServiceMock = new Mock<IEmailService>();
        _handler = new RegisterCommandHandler(_unitOfWorkMock.Object, _tokenServiceMock.Object, _emailServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsUserId()
    {
        var command = new RegisterCommand 
        { 
            UserName = "testuser", 
            Email = "new@test.com", 
            Password = "password123",
            FirstName = "testuser",
            LastName = "testuser"
        };
        var expectedUserId = 1;
        var confirmationToken = "confirmation-token";

        _unitOfWorkMock.Setup(u => u.Users.GetUserByUsernameAsync(command.UserName))
                      .ReturnsAsync((User)null);
        _unitOfWorkMock.Setup(u => u.Users.AddAsync(It.IsAny<User>()))
                      .Callback<User>(user => user.Id = expectedUserId)
                      .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                      .ReturnsAsync(1);
        _tokenServiceMock.Setup(t => t.GenerateAndSaveTokenAsync(
            expectedUserId, TokenAssignment.EmailConfirmation, TimeSpan.FromHours(24)))
                        .ReturnsAsync(confirmationToken);
        _emailServiceMock.Setup(e => e.SendEmailConfirmationAsync(It.IsAny<User>(), confirmationToken))
                        .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(expectedUserId, result);
        _unitOfWorkMock.Verify(u => u.Users.GetUserByUsernameAsync(command.UserName), Times.Once);
        _unitOfWorkMock.Verify(u => u.Users.AddAsync(It.IsAny<User>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        _tokenServiceMock.Verify(t => t.GenerateAndSaveTokenAsync(
            expectedUserId, TokenAssignment.EmailConfirmation, TimeSpan.FromHours(24)), Times.Once);
        _emailServiceMock.Verify(e => e.SendEmailConfirmationAsync(It.IsAny<User>(), confirmationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_UsernameAlreadyExists_ThrowsUnauthorizedException()
    {
        var command = new RegisterCommand 
        { 
            UserName = "existinguser", 
            Email = "test@test.com", 
            Password = "password123" 
        };
        var existingUser = new User { Id = 1, UserName = "existinguser" };

        _unitOfWorkMock.Setup(u => u.Users.GetUserByUsernameAsync(command.UserName))
                      .ReturnsAsync(existingUser);

        var exception = await Assert.ThrowsAsync<UnauthorizedException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal("Данный логин уже существует.", exception.Message);
        _unitOfWorkMock.Verify(u => u.Users.GetUserByUsernameAsync(command.UserName), Times.Once);
        _unitOfWorkMock.Verify(u => u.Users.AddAsync(It.IsAny<User>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_CreatesUserWithCorrectProperties()
    {
        var command = new RegisterCommand 
        { 
            UserName = "testuser", 
            Email = "new@test.com", 
            Password = "password123",
            FirstName = "testuser",
            LastName = "testuser"
        };
        User createdUser = null;
        var confirmationToken = "confirmation-token";

        _unitOfWorkMock.Setup(u => u.Users.GetUserByUsernameAsync(command.UserName))
                      .ReturnsAsync((User)null);
        _unitOfWorkMock.Setup(u => u.Users.AddAsync(It.IsAny<User>()))
                      .Callback<User>(user => 
                      {
                          createdUser = user;
                          user.Id = 1;
                      })
                      .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                      .ReturnsAsync(1);
        _tokenServiceMock.Setup(t => t.GenerateAndSaveTokenAsync(It.IsAny<int>(), It.IsAny<TokenAssignment>(), It.IsAny<TimeSpan>()))
                        .ReturnsAsync(confirmationToken);
        _emailServiceMock.Setup(e => e.SendEmailConfirmationAsync(It.IsAny<User>(), It.IsAny<string>()))
                        .Returns(Task.CompletedTask);

        await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(createdUser);
        Assert.Equal(command.UserName, createdUser.UserName);
        Assert.Equal(command.Email, createdUser.Email);
        Assert.False(createdUser.EmailConfirmed);
        Assert.True(createdUser.IsActive);
        Assert.Equal(UserRole.User, createdUser.Role);
        Assert.True(BCrypt.Net.BCrypt.Verify(command.Password, createdUser.PasswordHash));
        Assert.NotNull(createdUser.Profile);
        Assert.Equal(command.FirstName, createdUser.Profile.FirstName);
        Assert.Equal(command.LastName, createdUser.Profile.LastName);
    }
}