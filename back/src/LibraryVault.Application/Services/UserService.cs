using LibraryVault.Application.Interfaces.Repositories;
using LibraryVault.Application.Interfaces.Services;
using LibraryVault.Domain.Entities;

namespace LibraryVault.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<int> AddUserAsync(string name, string email, string password, bool isAdmin)
        {
            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
            {
                throw new Exception("User already exists with this email.");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Name = name,
                Email = email,
                PasswordHash = hashedPassword,
                IsAdmin = isAdmin
            };

            await _userRepository.AddAsync(user);
            return user.Id;
        }

        public Task<User> GetUserByIdAsync(int userId)
        {
            return _userRepository.GetByIdAsync(userId) ?? throw new Exception("User not found.");
        }

        public async Task UpdateUserAsync(int id, string name, string email, bool isAdmin)
        {
            var user = await _userRepository.GetByIdAsync(id) ?? throw new Exception("User not found.");

            user.Name = name;
            user.Email = email;
            user.IsAdmin = isAdmin;

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteAsync(id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }
    }
}