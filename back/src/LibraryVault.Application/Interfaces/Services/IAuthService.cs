using LibraryVault.Domain.Entities;

namespace LibraryVault.Application.Interfaces.Services
{
    public interface IAuthService
    {
        string GenerateToken(User user);
        Task<bool> ValidateCredentialsAsync(string email, string password);
    }
}