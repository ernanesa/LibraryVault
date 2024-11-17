using LibraryVault.API.Contracts.Requests;
using LibraryVault.API.Contracts.Responses;
using LibraryVault.Application.Interfaces.Services;

namespace LibraryVault.API.Endpoints
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this WebApplication app)
        {
            var userGroup = app.MapGroup("/users")
                .WithTags("Users");

            userGroup.MapGet("/users", async (IUserService userService) =>
            {
                var users = await userService.GetUsersAsync();
                var usersResponse = users.Select(user => new UserResponse(user));
                return Results.Ok(usersResponse);
            })
            .RequireAuthorization();

            userGroup.MapPost("/users", async (IUserService userService, CreateUserRequest request) =>
            {
                var userId = await userService.AddUserAsync(request.Name, request.Email, request.Password, request.IsAdmin);
                return Results.Created($"/users/{userId}", userId);
            })
            .RequireAuthorization();

            userGroup.MapPut("/users/{id:int}", async (IUserService userService, int id, UpdateUserRequest request) =>
            {
                var user = await userService.GetUserByIdAsync(id);
                if (user == null) return Results.NotFound("User not found.");
                if(user.Id != id) return Results.BadRequest("User ID does not match.");
                if(user.Email != request.Email)
                {
                    var existingUser = await userService.GetUserByEmailAsync(request.Email);
                    if (existingUser != null) return Results.Conflict("User already exists with this email.");
                }
                await userService.UpdateUserAsync(id, request.Name, request.Email, request.IsAdmin);
                return Results.NoContent();
            })
            .RequireAuthorization();

            userGroup.MapDelete("/users/{id:int}", async (IUserService userService, int id) =>
            {
                var user = await userService.GetUserByIdAsync(id);
                if (user == null) return Results.NotFound("User not found.");
                await userService.DeleteUserAsync(id);
                return Results.NoContent();
            })
            .RequireAuthorization();
        }
    }
}