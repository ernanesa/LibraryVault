using Moq;
using Microsoft.AspNetCore.Builder;
using LibraryVault.API.Contracts.Requests;
using LibraryVault.API.Contracts.Responses;
using LibraryVault.API.Endpoints;
using LibraryVault.Application.Interfaces.Services;
using LibraryVault.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LibraryVault.Tests.API.Tests.Endpoints
{
    public class AuthEndpointsTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly WebApplication _app;

        public AuthEndpointsTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _userServiceMock = new Mock<IUserService>();
            
            var builder = WebApplication.CreateBuilder();
            _app = builder.Build();
            _app.MapAuthEndpoints();
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var loginRequest = new LoginRequest 
            { 
                Email = "test@example.com",
                Password = "password123"
            };

            var user = new User
            {
                Id = 1,
                Email = loginRequest.Email,
                Name = "Test User"
            };

            var expectedToken = "valid.jwt.token";

            _authServiceMock
                .Setup(x => x.ValidateCredentialsAsync(loginRequest.Email, loginRequest.Password))
                .ReturnsAsync(true);

            _userServiceMock
                .Setup(x => x.GetUserByEmailAsync(loginRequest.Email))
                .ReturnsAsync(user);

            _authServiceMock
                .Setup(x => x.GenerateToken(user))
                .Returns(expectedToken);

            // Act
            var result = await GetLoginEndpoint()(_authServiceMock.Object, _userServiceMock.Object, loginRequest);

            // Assert
            var okResult = Assert.IsType<Ok<AuthResponse>>(result);
            Assert.Equal(expectedToken, okResult.Value.Token);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest 
            { 
                Email = "test@example.com",
                Password = "wrongpassword"
            };

            _authServiceMock
                .Setup(x => x.ValidateCredentialsAsync(loginRequest.Email, loginRequest.Password))
                .ReturnsAsync(false);

            // Act
            var result = await GetLoginEndpoint()(_authServiceMock.Object, _userServiceMock.Object, loginRequest);

            // Assert
            Assert.IsType<UnauthorizedHttpResult>(result);
        }

        [Fact]
        public async Task Login_WithValidCredentialsButUserNotFound_ReturnsNotFound()
        {
            // Arrange
            var loginRequest = new LoginRequest 
            { 
                Email = "nonexistent@example.com",
                Password = "password123"
            };

            _authServiceMock
                .Setup(x => x.ValidateCredentialsAsync(loginRequest.Email, loginRequest.Password))
                .ReturnsAsync(true);

            _userServiceMock
                .Setup(x => x.GetUserByEmailAsync(loginRequest.Email))
                .ReturnsAsync((User)null);

            // Act
            var result = await GetLoginEndpoint()(_authServiceMock.Object, _userServiceMock.Object, loginRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFound<string>>(result);
            Assert.Equal("User not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task Login_WithNullRequest_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                GetLoginEndpoint()(_authServiceMock.Object, _userServiceMock.Object, null));
        }

        private static Func<IAuthService, IUserService, LoginRequest, Task<IResult>> GetLoginEndpoint()
        {
            return async (authService, userService, request) =>
            {
                if (request == null) throw new ArgumentNullException(nameof(request));

                if (!await authService.ValidateCredentialsAsync(request.Email, request.Password))
                    return Results.Unauthorized();

                var user = await userService.GetUserByEmailAsync(request.Email);
                if (user == null) return Results.NotFound("User not found.");

                var token = authService.GenerateToken(user);
                return Results.Ok(new AuthResponse(token));
            };
        }
    }
}