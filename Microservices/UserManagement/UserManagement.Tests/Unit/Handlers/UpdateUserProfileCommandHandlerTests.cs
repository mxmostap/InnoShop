using Moq;
using UserManagement.Application.Commands;
using UserManagement.Application.Exceptions;
using UserManagement.Application.Handlers;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Repositories;

namespace UserManagement.Tests.Unit.Handlers;

public class UpdateUserProfileCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UpdateUserProfileCommandHandler _handler;
    
        public UpdateUserProfileCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new UpdateUserProfileCommandHandler(_unitOfWorkMock.Object);
        }
    
        [Fact]
        public async Task Handle_ValidCommand_ReturnsUpdatedProfile()
        {
            var command = new UpdateUserProfileCommand 
            { 
                UserId = 1, 
                FirstName = "NewFirstName", 
                LastName = "NewLastName" 
            };
            var profile = new Profile 
            { 
                Id = 1, 
                UserId = 1, 
                FirstName = "OldFirstName", 
                LastName = "OldLastName" 
            };
    
            _unitOfWorkMock.Setup(u => u.Profiles.GetByIdAsync(command.UserId))
                          .ReturnsAsync(profile);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                          .ReturnsAsync(1);
    
            var result = await _handler.Handle(command, CancellationToken.None);
    
            Assert.IsType<Profile>(result);
            Assert.Equal(command.FirstName, result.FirstName);
            Assert.Equal(command.LastName, result.LastName);
            Assert.Equal(command.UserId, result.UserId);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    
        [Fact]
        public async Task Handle_ProfileNotFound_ThrowsNotFoundException()
        {
            var command = new UpdateUserProfileCommand 
            { 
                UserId = 123, 
                FirstName = "test", 
                LastName = "test" 
            };
    
            _unitOfWorkMock.Setup(u => u.Profiles.GetByIdAsync(command.UserId))
                          .ReturnsAsync((Profile)null);
    
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal($"Пользователь с ID \"{command.UserId}\" не найден.", exception.Message);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }
    
        [Fact]
        public async Task Handle_UpdatesProfilePropertiesCorrectly()
        {
            var command = new UpdateUserProfileCommand 
            { 
                UserId = 1, 
                FirstName = "UpdatedFirstName", 
                LastName = "UpdatedLastName" 
            };
            var profile = new Profile 
            { 
                Id = 1, 
                UserId = 1, 
                FirstName = "OriginalFirstName", 
                LastName = "OriginalLastName" 
            };
    
            _unitOfWorkMock.Setup(u => u.Profiles.GetByIdAsync(command.UserId))
                          .ReturnsAsync(profile);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                          .ReturnsAsync(1);
    
            var result = await _handler.Handle(command, CancellationToken.None);
    
            Assert.Equal(command.FirstName, profile.FirstName);
            Assert.Equal(command.LastName, profile.LastName);
            Assert.Same(profile, result); // Проверяем, что возвращен тот же объект
        }
    
        [Fact]
        public async Task Handle_CallsRepositoryWithCorrectUserId()
        {
            var command = new UpdateUserProfileCommand 
            { 
                UserId = 5, 
                FirstName = "test", 
                LastName = "test" 
            };
            var profile = new Profile { Id = 1, UserId = 5 };
            int? actualUserId = null;
    
            _unitOfWorkMock.Setup(u => u.Profiles.GetByIdAsync(It.IsAny<int>()))
                          .Callback<int>(id => actualUserId = id)
                          .ReturnsAsync(profile);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                          .ReturnsAsync(1);
    
            await _handler.Handle(command, CancellationToken.None);
    
            Assert.Equal(command.UserId, actualUserId);
        }
}