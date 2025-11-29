using Moq;
using UserManagement.Application.Handlers;
using UserManagement.Application.Queries;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Repositories;

namespace UserManagement.Tests.Unit.Handlers;

public class GetAllUsersQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly GetAllUsersQueryHandler _handler;

    public GetAllUsersQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new GetAllUsersQueryHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsListOfUsers()
    {
        var users = new List<User>
        {
            new User { Id = 1, UserName = "user1", Email = "user1@test.com" },
            new User { Id = 2, UserName = "user2", Email = "user2@test.com" },
            new User { Id = 3, UserName = "user3", Email = "user3@test.com" }
        };
        
        _unitOfWorkMock.Setup(u => u.Users.GetAllAsync())
                      .ReturnsAsync(users);

        var result = await _handler.Handle(new GetAllUsersQuery(), CancellationToken.None);

        Assert.IsType<List<User>>(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("user1", result[0].UserName);
        Assert.Equal("user2", result[1].UserName);
        Assert.Equal("user3", result[2].UserName);
        _unitOfWorkMock.Verify(u => u.Users.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyDatabase_ReturnsEmptyList()
    {
        var emptyUsers = new List<User>();
        _unitOfWorkMock.Setup(u => u.Users.GetAllAsync())
                      .ReturnsAsync(emptyUsers);

        var result = await _handler.Handle(new GetAllUsersQuery(), CancellationToken.None);

        Assert.IsType<List<User>>(result);
        Assert.Empty(result);
        _unitOfWorkMock.Verify(u => u.Users.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_DatabaseError_ThrowsException()
    {
        _unitOfWorkMock.Setup(u => u.Users.GetAllAsync())
                      .ThrowsAsync(new Exception("Ошибка подключения к БД."));

        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(new GetAllUsersQuery(), CancellationToken.None));
        _unitOfWorkMock.Verify(u => u.Users.GetAllAsync(), Times.Once);
    }
}