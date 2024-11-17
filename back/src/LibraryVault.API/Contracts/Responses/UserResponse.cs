using LibraryVault.Domain.Entities;

namespace LibraryVault.API.Contracts.Responses;

public class UserResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public bool IsAdmin { get; set; }

    public UserResponse(User user)
    {
        Id = user.Id;
        Name = user.Name;
        Email = user.Email;
        IsAdmin = user.IsAdmin;
    }
}