using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserManagement.API.Controllers;
using UserManagement.Application.Commands;
using UserManagement.Application.Exceptions;
using UserManagement.Application.Queries;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;

namespace UserManagement.Tests.Unit.Controllers;

public class UserControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly UserController _userController;

    public UserControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _userController = new UserController(_mediatorMock.Object);
    }

    public class GetMethods : UserControllerTests
    {
        [Fact]
        public async Task GetAll_AdminRole_ReturnsUserList()
        {
            var users = new List<User>
            {
                new User { Id = 1, UserName = "user1", Email = "user1@test.com" },
                new User { Id = 2, UserName = "user2", Email = "user2@test.com" }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllUsersQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(users);

            var result = await _userController.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUsers = Assert.IsType<List<User>>(okResult.Value);
            Assert.Equal(2, returnedUsers.Count);
            Assert.Equal("user1", returnedUsers[0].UserName);
        }

        [Fact]
        public async Task GetAll_EmptyList_ReturnsEmptyList()
        {
            var emptyUsers = new List<User>();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllUsersQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyUsers);

            var result = await _userController.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUsers = Assert.IsType<List<User>>(okResult.Value);
            Assert.Empty(returnedUsers);
        }

        [Fact]
        public async Task GetAll_MediatorThrowsException_ThrowsException()
        {
            _mediatorMock.Setup(m => m.Send(
                    It.IsAny<GetAllUsersQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Ошибка БД."));

            await Assert.ThrowsAsync<Exception>(() => _userController.GetAll());
        }

        [Fact]
        public async Task GetUserById_ValidId_ReturnsUser()
        {
            var userId = 1;
            var user = new User { Id = userId, UserName = "testuser", Email = "test@test.com" };
            _mediatorMock
                .Setup(m => m.Send(It.Is<GetUserByIdQuery>(q => q.Id == userId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var result = await _userController.GetUserById(userId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<User>(okResult.Value);
            Assert.Equal(userId, returnedUser.Id);
            Assert.Equal("testuser", returnedUser.UserName);
        }

        [Fact]
        public async Task GetUserById_UserNotFound_ReturnsNullUser()
        {
            var userId = 123;
            _mediatorMock
                .Setup(m => m.Send(It.Is<GetUserByIdQuery>(q => q.Id == userId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            var result = await _userController.GetUserById(userId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Null(okResult.Value);
        }

        [Fact]
        public async Task GetUserById_DbConnectionError_ThrowsException()
        {
            var userId = 1;
            _mediatorMock
                .Setup(m => m.Send(It.Is<GetUserByIdQuery>(q => q.Id == userId),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Ошибка БД."));

            await Assert.ThrowsAsync<Exception>(() => _userController.GetUserById(userId));
        }

        [Fact]
        public async Task GetAllUsers_UserRole_ReturnsUsersList()
        {
            var users = new List<User>
            {
                new User { Id = 1, UserName = "user1", Email = "user1@test.com", Role = UserRole.User },
                new User { Id = 2, UserName = "user2", Email = "user2@test.com", Role = UserRole.User }
            };

            _mediatorMock.Setup(m =>
                    m.Send(It.Is<GetUsersByRoleQuery>(q => q.Role == "User"),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(users);

            var result = await _userController.GetAllUsers();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUsers = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
            Assert.Equal(2, returnedUsers.Count());
            Assert.All(returnedUsers, u => Assert.Equal(UserRole.User, u.Role));
        }

        [Fact]
        public async Task GetAllUsers_NoUsersFound_ReturnsEmptyList()
        {
            var emptyUsers = new List<User>();
            _mediatorMock.Setup(m =>
                    m.Send(It.Is<GetUsersByRoleQuery>(q => q.Role == "User"),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyUsers);

            var result = await _userController.GetAllUsers();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUsers = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
            Assert.Empty(returnedUsers);
        }

        [Fact]
        public async Task GetAllUsers_DatabaseError_ThrowsException()
        {
            _mediatorMock.Setup(m =>
                    m.Send(It.Is<GetUsersByRoleQuery>(q => q.Role == "User"),
                        It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Ошибка БД."));

            await Assert.ThrowsAsync<Exception>(() => _userController.GetAllUsers());
        }
    }

    public class PostMethods : UserControllerTests
    {
        [Fact]
        public async Task ResetPassword_ValidCommand_ReturnsSuccessMessage()
        {
            var command = new ResetPasswordCommand { Email = "test@test.com" };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(default(MediatR.Unit));

            var result = await _userController.ResetPassword(command);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value.GetType().GetProperty("message").GetValue(okResult.Value) as string;
            Assert.Contains("Если пользователь с таким Email существует", response);
        }

        [Fact]
        public async Task ResetPassword_NotFoundException_ThrowsException()
        {
            var command = new ResetPasswordCommand { Email = "test@test.com" };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Ошибка получения запроса."));

            await Assert.ThrowsAsync<NotFoundException>(() => _userController.ResetPassword(command));
        }
        
        [Fact]
        public async Task ResetPasswordConfirm_Successful_ReturnsOk()
        {
            var command = new ResetPasswordConfirmCommand();
            var successResult = ResetPasswordConfirmCommandResult.Successful();
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(successResult);

            var result = await _userController.ResetPasswordConfirm(command);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResult = Assert.IsType<ResetPasswordConfirmCommandResult>(okResult.Value);
            Assert.True(returnedResult.Success);
            Assert.Equal("Пароль успешно изменен", returnedResult.Message);
        }
        
        [Theory]
        [InlineData("Неверный токен или Email.")]
        [InlineData("Неверный или просроченный токен.")]
        public async Task ResetPasswordConfirm_FailedScenarios_ReturnsBadRequest(string errorMessage)
        {
            var command = new ResetPasswordConfirmCommand();
            var failedResult = ResetPasswordConfirmCommandResult.Failed(errorMessage);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(failedResult);

            var result = await _userController.ResetPasswordConfirm(command);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnedResult = Assert.IsType<ResetPasswordConfirmCommandResult>(badRequestResult.Value);
            Assert.False(returnedResult.Success);
            Assert.Equal(errorMessage, returnedResult.Message);
        }
        
        [Fact]
        public async Task ConfirmEmail_Successful_ReturnsOk()
        {
            var command = new ConfirmEmailCommand();
            var successResult = ConfirmEmailCommandResult.Successful();
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(successResult);

            var result = await _userController.ConfirmEmail(command);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResult = Assert.IsType<ConfirmEmailCommandResult>(okResult.Value);
            Assert.True(returnedResult.Success);
            Assert.Equal("Email успешно подтвержден", returnedResult.Message);
        }
        
        [Theory]
        [InlineData("Пользователь не найден.")]
        [InlineData("Email уже подтвержден.")]
        [InlineData("Неверный или просроченный токен")]
        public async Task ConfirmEmail_FailedScenarios_ReturnsBadRequest(string errorMessage)
        {
            var command = new ConfirmEmailCommand();
            var failedResult = ConfirmEmailCommandResult.Failed(errorMessage);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(failedResult);

            var result = await _userController.ConfirmEmail(command);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnedResult = Assert.IsType<ConfirmEmailCommandResult>(badRequestResult.Value);
            Assert.False(returnedResult.Success);
            Assert.Equal(errorMessage, returnedResult.Message);
        }
    }

    public class PutPatchMethods : UserControllerTests
    {
        [Fact]
        public async Task DeactivateUser_ValidId_ReturnsOk()
        {
            var userId = 1;
            _mediatorMock.Setup(m => m.Send(It.Is<DeactivateUserCommand>(c => c.UserId == userId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(default(MediatR.Unit));

            var result = await _userController.DeactivateUser(userId);

            Assert.IsType<OkResult>(result);
            _mediatorMock.Verify(m => m.Send(It.Is<DeactivateUserCommand>(c => c.UserId == userId),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeactivateUser_UserNotFound_ThrowsNotFoundException()
        {
            var userId = 1234;
            var exceptionMessage = $"Пользователь с ID \"{userId}\" не найден.";
            _mediatorMock.Setup(m =>
                    m.Send(It.Is<DeactivateUserCommand>(c => c.UserId == userId),
                        It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException(exceptionMessage));

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _userController.DeactivateUser(userId));
            Assert.Equal(exceptionMessage, exception.Message);
            _mediatorMock.Verify(m => m.Send(It.Is<DeactivateUserCommand>(c => c.UserId == userId),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeactivateUser_DatabaseError_ThrowsException()
        {
            var userId = 1;
            _mediatorMock.Setup(m => m.Send(It.Is<DeactivateUserCommand>(c => c.UserId == userId),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Ошибка БД."));

            await Assert.ThrowsAsync<Exception>(() => _userController.DeactivateUser(userId));
        }

        [Fact]
        public async Task DeactivateUser_ValidCommandSentToMediator()
        {
            var userId = 1;
            DeactivateUserCommand capturedCommand = null;
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeactivateUserCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<MediatR.Unit>, CancellationToken>((cmd, _) =>
                    capturedCommand = (DeactivateUserCommand)cmd)
                .ReturnsAsync(default(MediatR.Unit));

            await _userController.DeactivateUser(userId);

            Assert.NotNull(capturedCommand);
            Assert.Equal(userId, capturedCommand.UserId);
        }

        [Fact]
        public async Task UpdateUser_ValidCommand_ReturnsUpdatedUser()
        {
            var command = new UpdateUserCommand { UserId = 1, UserName = "newusername", Email = "new@test.com" };
            var updatedUser = new User { Id = 1, UserName = "newusername", Email = "new@test.com" };

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedUser);

            var result = await _userController.UpdateUser(command);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<User>(okResult.Value);
            Assert.Equal(command.UserId, returnedUser.Id);
            Assert.Equal(command.UserName, returnedUser.UserName);
            Assert.Equal(command.Email, returnedUser.Email);
        }

        [Fact]
        public async Task UpdateUser_UserNotFound_ThrowsNotFoundException()
        {
            var command = new UpdateUserCommand { UserId = 999, UserName = "nonexistent" };
            var exceptionMessage = $"Пользователь с ID \"{command.UserId}\" не найден.";

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException(exceptionMessage));

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _userController.UpdateUser(command));
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Fact]
        public async Task UpdateUser_DatabaseError_ThrowsException()
        {
            var command = new UpdateUserCommand { UserId = 1, UserName = "testuser" };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Ошибка БД."));

            await Assert.ThrowsAsync<Exception>(() => _userController.UpdateUser(command));
        }

        [Fact]
        public async Task UpdateUser_ValidCommandSentToMediator()
        {
            var command = new UpdateUserCommand { UserId = 1, UserName = "testuser", Email = "test@test.com" };
            UpdateUserCommand capturedCommand = null;

            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
                .Callback<IRequest<User>, CancellationToken>((cmd, _) => capturedCommand = (UpdateUserCommand)cmd)
                .ReturnsAsync(new User { Id = 1 });

            await _userController.UpdateUser(command);

            Assert.NotNull(capturedCommand);
            Assert.Equal(command.UserId, capturedCommand.UserId);
            Assert.Equal(command.UserName, capturedCommand.UserName);
            Assert.Equal(command.Email, capturedCommand.Email);
        }

        [Fact]
        public async Task UpdateUserProfile_ValidCommand_ReturnsUpdatedProfile()
        {
            var command = new UpdateUserProfileCommand { UserId = 1, FirstName = "John", LastName = "Doe" };
            var updatedProfile = new Profile { UserId = 1, FirstName = "John", LastName = "Doe" };

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedProfile);

            var result = await _userController.UpdateUserProfile(command);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProfile = Assert.IsType<Profile>(okResult.Value);
            Assert.Equal(command.UserId, returnedProfile.UserId);
            Assert.Equal(command.FirstName, returnedProfile.FirstName);
            Assert.Equal(command.LastName, returnedProfile.LastName);
        }

        [Fact]
        public async Task UpdateUserProfile_UserNotFound_ThrowsNotFoundException()
        {
            var command = new UpdateUserProfileCommand { UserId = 999, FirstName = "John" };
            var exceptionMessage = $"Пользователь с ID \"{command.UserId}\" не найден.";

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException(exceptionMessage));

            var exception =
                await Assert.ThrowsAsync<NotFoundException>(() => _userController.UpdateUserProfile(command));
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Fact]
        public async Task UpdateUserProfile_DatabaseError_ThrowsException()
        {
            var command = new UpdateUserProfileCommand { UserId = 1, FirstName = "John" };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Ошибка БД."));

            await Assert.ThrowsAsync<Exception>(() => _userController.UpdateUserProfile(command));
        }

        [Fact]
        public async Task UpdateUserProfile_ValidCommandSentToMediator()
        {
            var command = new UpdateUserProfileCommand { UserId = 1, FirstName = "John", LastName = "Doe" };
            UpdateUserProfileCommand capturedCommand = null;

            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserProfileCommand>(), It.IsAny<CancellationToken>()))
                .Callback<IRequest<Profile>, CancellationToken>((cmd, _) =>
                    capturedCommand = (UpdateUserProfileCommand)cmd)
                .ReturnsAsync(new Profile { UserId = 1 });

            await _userController.UpdateUserProfile(command);

            Assert.NotNull(capturedCommand);
            Assert.Equal(command.UserId, capturedCommand.UserId);
            Assert.Equal(command.FirstName, capturedCommand.FirstName);
            Assert.Equal(command.LastName, capturedCommand.LastName);
        }
    }

    public class DeleteMethods : UserControllerTests
    {
        [Fact]
        public async Task DeleteUser_ValidId_ReturnsOk()
        {
            var userId = 1;
            _mediatorMock.Setup(m =>
                    m.Send(It.Is<DeleteUserCommand>(c => c.UserId == userId),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(default(MediatR.Unit));

            var result = await _userController.DeleteUser(userId);

            Assert.IsType<OkResult>(result);
            _mediatorMock.Verify(
                m => m.Send(It.Is<DeleteUserCommand>(c => c.UserId == userId),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task DeleteUser_UserNotFound_ThrowsNotFoundException()
        {
            var userId = 12;
            var exceptionMessage = $"Пользователь с ID \"{userId}\" не найден.";
            _mediatorMock.Setup(m =>
                    m.Send(It.Is<DeleteUserCommand>(c => c.UserId == userId),
                        It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException(exceptionMessage));

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _userController.DeleteUser(userId));
            Assert.Equal(exceptionMessage, exception.Message);
            _mediatorMock.Verify(
                m => m.Send(It.Is<DeleteUserCommand>(c => c.UserId == userId),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task DeleteUser_DatabaseError_ThrowsException()
        {
            var userId = 1;
            _mediatorMock.Setup(m =>
                    m.Send(It.Is<DeleteUserCommand>(c => c.UserId == userId),
                        It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Ошибка БД."));

            await Assert.ThrowsAsync<Exception>(() => _userController.DeleteUser(userId));
        }
    }
}