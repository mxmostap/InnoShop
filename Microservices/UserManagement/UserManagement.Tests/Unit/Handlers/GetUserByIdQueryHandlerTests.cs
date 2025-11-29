using Moq;
using UserManagement.Application.Handlers;
using UserManagement.Application.Queries;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Repositories;

namespace UserManagement.Tests.Unit.Handlers;

public class GetUserByIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly GetUserByIdQueryHandler _handler;

    public GetUserByIdQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new GetUserByIdQueryHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidId_ReturnsUser()
    {
        var userId = 1;
        var query = new GetUserByIdQuery { Id = userId };
        var user = new User { Id = userId, UserName = "testuser", Email = "test@test.com" };
        
        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(userId))
                      .ReturnsAsync(user);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.IsType<User>(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal("testuser", result.UserName);
        Assert.Equal("test@test.com", result.Email);
        _unitOfWorkMock.Verify(u => u.Users.GetByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsNull()
    {
        var userId = 123;
        var query = new GetUserByIdQuery { Id = userId };
        
        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(userId))
                      .ReturnsAsync((User)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
        _unitOfWorkMock.Verify(u => u.Users.GetByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task Handle_DatabaseError_ThrowsException()
    {
        var userId = 1;
        var query = new GetUserByIdQuery { Id = userId };
        
        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(userId))
                      .ThrowsAsync(new Exception("Ошибка БД."));

        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        _unitOfWorkMock.Verify(u => u.Users.GetByIdAsync(userId), Times.Once);
    }
}