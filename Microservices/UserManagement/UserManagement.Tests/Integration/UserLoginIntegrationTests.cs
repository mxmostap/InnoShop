using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using UserManagement.Application.Commands;
using UserManagement.Application.DTOs;
using UserManagement.Domain.Enums;
using UserManagement.Tests.Integration.Common;
using Xunit.Abstractions;

namespace UserManagement.Tests.Integration;

public class UserLoginIntegrationTests : BaseIntegrationTest
{

    public UserLoginIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Login_ShouldReturnAuthResponse_WhenLoginSuccessful()
    {
        var userName = "testUser";
        var password = "test";
        var email = "test@test.com";
        var firstName = "Test";
        var lastName = "User";

        var registerCommand = new RegisterCommand
        {
            UserName = userName,
            Email = email,
            Password = password,
            FirstName = firstName,
            LastName = lastName
        };

        await Client.PostAsJsonAsync("/api/auth/register", registerCommand);

        var loginCommand = new LoginCommand(userName, password);

        var response = await Client.PostAsJsonAsync("/api/auth/login", loginCommand);
        
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(authResponse);

        Assert.NotEqual(0, authResponse.Id);
        Assert.Equal(userName, authResponse.UserName);
        Assert.Equal(email, authResponse.Email);
        Assert.Equal(firstName, authResponse.FirstName);
        Assert.Equal(lastName, authResponse.LastName);
        Assert.NotNull(authResponse.JwtToken);
        Assert.NotEmpty(authResponse.JwtToken);
        Assert.True(Enum.IsDefined(typeof(UserRole), authResponse.Role));
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenInvalidPassword()
    {
        var userName = "testUser";
        var correctPassword = "correctPassword";
        var wrongPassword = "wrongPassword";
        var email = "test@test.com";
        var firstName = "Test";
        var lastName = "User";

        var registerCommand = new RegisterCommand
        {
            UserName = userName,
            Email = email,
            Password = correctPassword,
            FirstName = firstName,
            LastName = lastName
        };

        await Client.PostAsJsonAsync("/api/auth/register", registerCommand);

        var loginCommand = new LoginCommand(userName, wrongPassword);

        var response = await Client.PostAsJsonAsync("/api/auth/login", loginCommand);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        var errorContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Неверный пароль", errorContent);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenUserNotFound()
    {
        var userName = "nonExistentUser";
        var password = "anyPassword";

        var loginCommand = new LoginCommand(userName, password);

        var response = await Client.PostAsJsonAsync("/api/auth/login", loginCommand);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        var errorContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Неверные учетные данные", errorContent);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenInvalidUsername()
    {
        var correctUserName = "correctUser";
        var wrongUserName = "wrongUser";
        var password = "testPassword";

        var registerCommand = new RegisterCommand
        {
            UserName = correctUserName,
            Email = "test@test.com",
            Password = password,
            FirstName = "Test",
            LastName = "User"
        };

        await Client.PostAsJsonAsync("/api/auth/register", registerCommand);

        var loginCommand = new LoginCommand(wrongUserName, password);

        var response = await Client.PostAsJsonAsync("/api/auth/login", loginCommand);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        var errorContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Неверные учетные данные", errorContent);
    }
}