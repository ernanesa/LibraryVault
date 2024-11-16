using LibraryVault.Domain.Entities;

namespace LibraryVault.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetUsersAsync();
        // Task<int> RegisterUserAsync(string name, string email, string password, bool isAdmin);
        Task UpdateUserAsync(int id, string name, string email, bool isAdmin);
        Task DeleteUserAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<int> AddUserAsync(string name, string email, string password, bool isAdmin);
        Task<User> GetUserByIdAsync(int userId);
    }
}