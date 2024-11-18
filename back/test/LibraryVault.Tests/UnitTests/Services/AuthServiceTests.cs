using FluentAssertions;
using LibraryVault.Application.Interfaces;
using LibraryVault.Application.Interfaces.Repositories;
using LibraryVault.Application.Interfaces.Services;
using LibraryVault.Application.Services;
using LibraryVault.Domain.Entities;
using Moq;

namespace LibraryVault.Tests.UnitTests.Services;

public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IJwtTokenGenerator> _jwtTokenGenerator;
        private readonly IAuthService _authService;

        public AuthServiceTests()
        {
            _userRepository = new Mock<IUserRepository>();
            _jwtTokenGenerator = new Mock<IJwtTokenGenerator>();
            _authService = new AuthService(_userRepository.Object, _jwtTokenGenerator.Object);
        }

        [Fact]
        public async Task ValidateCredentials_WithValidCredentials_ReturnsTrue()
        {
            // Arrange
            var email = "test@example.com";
            var password = "TestPassword123!";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User 
            { 
                Email = email,
                PasswordHash = hashedPassword
            };

            _userRepository.Setup(r => r.GetByEmailAsync(email))
                          .ReturnsAsync(user);

            // Act
            var result = await _authService.ValidateCredentialsAsync(email, password);

            // Assert
            result.Should().BeTrue();
            _userRepository.Verify(r => r.GetByEmailAsync(email), Times.Once);
        }

        [Fact]
        public async Task ValidateCredentials_WithInvalidPassword_ReturnsFalse()
        {
            // Arrange
            var email = "test@example.com";
            var correctPassword = "TestPassword123!";
            var wrongPassword = "WrongPassword123!";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(correctPassword);

            var user = new User 
            { 
                Email = email,
                PasswordHash = hashedPassword
            };

            _userRepository.Setup(r => r.GetByEmailAsync(email))
                          .ReturnsAsync(user);

            // Act
            var result = await _authService.ValidateCredentialsAsync(email, wrongPassword);

            // Assert
            result.Should().BeFalse();
            _userRepository.Verify(r => r.GetByEmailAsync(email), Times.Once);
        }

        [Fact]
        public async Task ValidateCredentials_WithNonexistentUser_ReturnsFalse()
        {
            // Arrange
            var email = "nonexistent@example.com";
            var password = "TestPassword123!";

            _userRepository.Setup(r => r.GetByEmailAsync(email))
                          .ReturnsAsync((User)null);

            // Act
            var result = await _authService.ValidateCredentialsAsync(email, password);

            // Assert
            result.Should().BeFalse();
            _userRepository.Verify(r => r.GetByEmailAsync(email), Times.Once);
        }

        [Fact]
        public void GenerateToken_WithValidUser_ReturnsToken()
        {
            // Arrange
            var user = new User 
            { 
                Id = 1,
                Email = "test@example.com",
                Name = "Test User"
            };
            var expectedToken = "test-jwt-token";

            _jwtTokenGenerator.Setup(g => g.GenerateToken(user))
                             .Returns(expectedToken);

            // Act
            var result = _authService.GenerateToken(user);

            // Assert
            result.Should().Be(expectedToken);
            _jwtTokenGenerator.Verify(g => g.GenerateToken(user), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task ValidateCredentials_WithInvalidEmail_ReturnsFalse(string invalidEmail)
        {
            // Arrange
            var password = "TestPassword123!";

            // Act
            var result = await _authService.ValidateCredentialsAsync(invalidEmail, password);

            // Assert
            result.Should().BeFalse();
            _userRepository.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ValidateCredentials_WithEmptyPassword_ReturnsFalse(string invalidPassword)
        {
            // Arrange
            var email = "test@example.com";
            var validPassword = "TestPassword123!";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(validPassword);

            var user = new User 
            { 
                Email = email,
                PasswordHash = hashedPassword
            };

            _userRepository.Setup(r => r.GetByEmailAsync(email))
                          .ReturnsAsync(user);

            // Act
            var result = await _authService.ValidateCredentialsAsync(email, invalidPassword);

            // Assert
            result.Should().BeFalse();
            _userRepository.Verify(r => r.GetByEmailAsync(email), Times.Once);
        }

        [Fact]
        public async Task ValidateCredentials_WhenRepositoryThrowsException_ThrowsSameException()
        {
            // Arrange
            var email = "test@example.com";
            var password = "TestPassword123!";
            var expectedException = new Exception("Database connection error");

            _userRepository.Setup(r => r.GetByEmailAsync(email))
                          .ThrowsAsync(expectedException);

            // Act
            var act = () => _authService.ValidateCredentialsAsync(email, password);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage(expectedException.Message);
            _userRepository.Verify(r => r.GetByEmailAsync(email), Times.Once);
        }
    }
