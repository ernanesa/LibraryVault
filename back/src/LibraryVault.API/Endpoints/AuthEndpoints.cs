using LibraryVault.API.Contracts.Requests;
using LibraryVault.API.Contracts.Responses;
using LibraryVault.Application.Interfaces.Services;

namespace LibraryVault.API.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this WebApplication app)
        {
            app.MapPost("/auth/login", async (IAuthService authService, IUserService userService, LoginRequest request) =>
            {
                if (!await authService.ValidateCredentialsAsync(request.Email, request.Password))
                    return Results.Unauthorized();

                var user = await userService.GetUserByEmailAsync(request.Email);
                if (user == null) return Results.NotFound("User not found.");

                var token = authService.GenerateToken(user);
                return Results.Ok(new AuthResponse(token));
            })
            .WithTags("Auth")
            .AllowAnonymous();
            
        }
    }
}