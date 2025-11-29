using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserManagement.API.Controllers;
using UserManagement.Application.Commands;
using UserManagement.Application.DTOs;
using UserManagement.Application.Exceptions;
using UserManagement.Domain.Enums;

namespace UserManagement.Tests.Unit.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _authController = new AuthController(_mediatorMock.Object);
    }

    [Fact]
    public async Task Register_ValidCommand_ReturnsOkResultWithIserId()
    {
        var expectedUserId = 12;
        var command = new RegisterCommand();
        _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUserId);

        var result = await _authController.Register(command);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedUserId, okResult.Value);
    }

    [Fact]
    public async Task Register_MediatorThrowsException_ThrowsException()
    {
        var command = new RegisterCommand();
        _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedException("Данный логин уже существует."));

        await Assert.ThrowsAsync<UnauthorizedException>(() => _authController.Register(command));
    }

    [Fact]
    public async Task Login_ValidCommand_ReturnsOkResultWithAuthResponse()
    {
        var expectedAuthResponse = new AuthResponse
        {
            Id = 12,
            UserName = "testUserName",
            Email = "test@example.com",
            FirstName = "testName",
            LastName = "testName",
            JwtToken = "jwtToken",
            Role = UserRole.User
        };
        var command = new LoginCommand("testUser", "testPassword");
        _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedAuthResponse);

        var result = await _authController.Login(command);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var authResponse = Assert.IsType<AuthResponse>(okResult.Value);
        Assert.Equal(expectedAuthResponse.Id, authResponse.Id);
        Assert.Equal(expectedAuthResponse.UserName, authResponse.UserName);
        Assert.Equal(expectedAuthResponse.Email, authResponse.Email);
        Assert.Equal(expectedAuthResponse.FirstName, authResponse.FirstName);
        Assert.Equal(expectedAuthResponse.LastName, authResponse.LastName);
        Assert.Equal(expectedAuthResponse.JwtToken, authResponse.JwtToken);
        Assert.Equal(expectedAuthResponse.Role, authResponse.Role);
    }

    [Theory]
    [InlineData("Неверные учетные данные.")]
    [InlineData("Неверный пароль.")]
    public async Task Login_MediatorThrowsUnauthorizedAccessException_ThrowsException(string errorMessage)
    {
        var command = new LoginCommand("testUser", "testPassword");
        _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException(errorMessage));

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authController.Login(command));
        Assert.Equal(errorMessage, exception.Message);
    }

    [Fact]
    public void Constructor_WithMediator_InitializesController()
    {
        var mediator = new Mock<IMediator>().Object;

        var controller = new AuthController(mediator);

        Assert.NotNull(controller);
    }
}