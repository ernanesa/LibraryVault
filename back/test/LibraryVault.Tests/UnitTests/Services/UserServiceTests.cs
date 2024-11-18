using Moq;
using LibraryVault.Application.Interfaces.Repositories;
using LibraryVault.Domain.Entities;
using LibraryVault.Application.Services;

namespace LibraryVault.Tests.UnitTests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _userService = new UserService(_mockUserRepository.Object);
        }

        [Fact]
        public async Task GetUsersAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var expectedUsers = new List<User>
            {
                new User { Id = 1, Name = "User1", Email = "user1@example.com", IsAdmin = false },
                new User { Id = 2, Name = "User2", Email = "user2@example.com", IsAdmin = true }
            };
            _mockUserRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(expectedUsers);

            // Act
            var result = await _userService.GetUsersAsync();

            // Assert
            Assert.Equal(expectedUsers, result);
            _mockUserRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task AddUserAsync_ShouldCreateUser_WhenEmailDoesNotExist()
        {
            // Arrange
            var name = "New User";
            var email = "newuser@example.com";
            var password = "password123";
            var isAdmin = false;

            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(email))
                .ReturnsAsync((User)null);

            User savedUser = null;
            _mockUserRepository.Setup(repo => repo.AddAsync(It.IsAny<User>()))
                .Callback<User>(user => savedUser = user)
                .Returns(Task.CompletedTask);

            // Act
            await _userService.AddUserAsync(name, email, password, isAdmin);

            // Assert
            Assert.NotNull(savedUser);
            Assert.Equal(name, savedUser.Name);
            Assert.Equal(email, savedUser.Email);
            Assert.Equal(isAdmin, savedUser.IsAdmin);
            _mockUserRepository.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task AddUserAsync_ShouldThrowException_WhenEmailExists()
        {
            // Arrange
            var existingUser = new User { Email = "existing@example.com" };
            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(existingUser.Email))
                .ReturnsAsync(existingUser);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _userService.AddUserAsync("name", existingUser.Email, "password", false));
            _mockUserRepository.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var expectedUser = new User { Id = 1, Name = "User1", Email = "user1@example.com", IsAdmin = false };
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(expectedUser);

            // Act
            var result = await _userService.GetUserByIdAsync(1);

            // Assert
            Assert.Equal(expectedUser, result);
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateUser_WhenUserExists()
        {
            // Arrange
            var existingUser = new User 
            { 
                Id = 1, 
                Name = "Old Name", 
                Email = "old@example.com", 
                IsAdmin = false 
            };
            var newName = "Updated Name";
            var newEmail = "updated@example.com";
            var newIsAdmin = true;

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingUser);
            _mockUserRepository.Setup(repo => repo.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            // Act
            await _userService.UpdateUserAsync(1, newName, newEmail, newIsAdmin);

            // Assert
            Assert.Equal(newName, existingUser.Name);
            Assert.Equal(newEmail, existingUser.Email);
            Assert.Equal(newIsAdmin, existingUser.IsAdmin);
            _mockUserRepository.Verify(repo => repo.UpdateAsync(existingUser), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _userService.UpdateUserAsync(1, "name", "email@example.com", false));
            _mockUserRepository.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldCallRepository()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            await _userService.DeleteUserAsync(1);

            // Assert
            _mockUserRepository.Verify(repo => repo.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetUserByEmailAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var expectedUser = new User 
            { 
                Id = 1, 
                Name = "User1", 
                Email = "user1@example.com", 
                IsAdmin = false 
            };
            _mockUserRepository.Setup(repo => repo.GetByEmailAsync("user1@example.com"))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _userService.GetUserByEmailAsync("user1@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUser, result);
            _mockUserRepository.Verify(repo => repo.GetByEmailAsync("user1@example.com"), Times.Once);
        }

        [Fact]
        public async Task GetUserByEmailAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.GetByEmailAsync("nonexistent@example.com"))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetUserByEmailAsync("nonexistent@example.com");

            // Assert
            Assert.Null(result);
            _mockUserRepository.Verify(repo => repo.GetByEmailAsync("nonexistent@example.com"), Times.Once);
        }
    }
}