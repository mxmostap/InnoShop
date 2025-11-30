using System.Net;
using System.Net.Http.Json;
using UserManagement.Application.Commands;
using UserManagement.Tests.Integration.Common;

namespace UserManagement.Tests.Integration;

public class UserRegistrationIntegrationTests : BaseIntegrationTest
{
    public UserRegistrationIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Register_ShouldReturnUserId_WhenRegistrationSuccessful()
    {
        var command = new RegisterCommand
        {
            UserName = "testUser",
            Email = "test@test.com",
            Password = "testPassword",
            FirstName = "test",
            LastName = "test"
        };

        var response = await Client.PostAsJsonAsync("/api/auth/register", command);
        
        response.EnsureSuccessStatusCode();

        var user = DbContext.Users
            .FirstOrDefault(u => u.UserName == command.UserName);

        Assert.NotNull(user);
        Assert.Equal(command.UserName, user.UserName);
        Assert.Equal(command.Email, user.Email);
    }

    [Fact]
    public async Task Register_ShouldThrowUnauthorizedException_WhenUsernameAlreadyExists()
    {
        var existingUsername = "existingUser";

        var firstCommand = new RegisterCommand
        {
            UserName = existingUsername,
            Email = "test1@test.com",
            Password = "testPassword1",
            FirstName = "test1",
            LastName = "test1"
        };

        var firstResponse = await Client.PostAsJsonAsync("/api/auth/register", firstCommand);
        firstResponse.EnsureSuccessStatusCode();

        var secondCommand = new RegisterCommand
        {
            UserName = existingUsername,
            Email = "test2@test.com",
            Password = "testPassword2",
            FirstName = "test2",
            LastName = "test2"
        };

        var secondResponse = await Client.PostAsJsonAsync("/api/auth/register", secondCommand);

        Assert.Equal(HttpStatusCode.Unauthorized, secondResponse.StatusCode);

        var errorContent = await secondResponse.Content.ReadAsStringAsync();
        Assert.Contains("Данный логин уже существует.", errorContent);
    }
}