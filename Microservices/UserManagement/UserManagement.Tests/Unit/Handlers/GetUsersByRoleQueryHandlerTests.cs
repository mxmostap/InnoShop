using Moq;
using UserManagement.Application.Handlers;
using UserManagement.Application.Queries;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;
using UserManagement.Domain.Repositories;

namespace UserManagement.Tests.Unit.Handlers;

public class GetUsersByRoleQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly GetUsersByRoleQueryHandler _handler;

    public GetUsersByRoleQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new GetUsersByRoleQueryHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRole_ReturnsUsers()
    {
        var role = "User";
        var query = new GetUsersByRoleQuery(role);
        var users = new List<User>
        {
            new User { Id = 1, UserName = "user1", Email = "user1@test.com", Role = UserRole.User },
            new User { Id = 2, UserName = "user2", Email = "user2@test.com", Role = UserRole.User }
        };
        
        _unitOfWorkMock.Setup(u => u.Users.GetUsersByRoleAsync(role))
                      .ReturnsAsync(users);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.IsAssignableFrom<IEnumerable<User>>(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, u => Assert.Equal(UserRole.User, u.Role));
        _unitOfWorkMock.Verify(u => u.Users.GetUsersByRoleAsync(role), Times.Once);
    }

    [Fact]
    public async Task Handle_NoUsersWithRole_ReturnsEmptyEnumerable()
    {
        var role = "Admin";
        var query = new GetUsersByRoleQuery(role);
        var emptyUsers = new List<User>();
        
        _unitOfWorkMock.Setup(u => u.Users.GetUsersByRoleAsync(role))
                      .ReturnsAsync(emptyUsers);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.IsAssignableFrom<IEnumerable<User>>(result);
        Assert.Empty(result);
        _unitOfWorkMock.Verify(u => u.Users.GetUsersByRoleAsync(role), Times.Once);
    }

    [Fact]
    public async Task Handle_DatabaseError_ThrowsException()
    {
        var role = "User";
        var query = new GetUsersByRoleQuery(role);
        
        _unitOfWorkMock.Setup(u => u.Users.GetUsersByRoleAsync(role))
                      .ThrowsAsync(new Exception("Ошибка БД."));

        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        _unitOfWorkMock.Verify(u => u.Users.GetUsersByRoleAsync(role), Times.Once);
    }

    [Theory]
    [InlineData("User")]
    [InlineData("Admin")]
    public async Task Handle_CallsRepositoryWithCorrectRole(string role)
    {
        var query = new GetUsersByRoleQuery(role);
        var userRole = role == "User" ? UserRole.User : UserRole.Admin;
        var users = new List<User> { new User { Id = 1, UserName = "testUser", Role = userRole } };
        
        _unitOfWorkMock.Setup(u => u.Users.GetUsersByRoleAsync(role))
                      .ReturnsAsync(users);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(userRole, result.First().Role);
        _unitOfWorkMock.Verify(u => u.Users.GetUsersByRoleAsync(role), Times.Once);
    }
}