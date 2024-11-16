using LibraryVault.API.Contracts.Requests;
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
                return Results.Ok(users);
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
                await userService.UpdateUserAsync(id, request.Name, request.Email, request.IsAdmin);
                return Results.NoContent();
            })
            .RequireAuthorization();

            userGroup.MapDelete("/users/{id:int}", async (IUserService userService, int id) =>
            {
                await userService.DeleteUserAsync(id);
                return Results.NoContent();
            })
            .RequireAuthorization();
        }
    }
}