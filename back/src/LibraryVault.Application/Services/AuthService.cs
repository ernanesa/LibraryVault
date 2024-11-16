using LibraryVault.Application.Interfaces;
using LibraryVault.Domain.Entities;
using LibraryVault.Application.Interfaces.Repositories;
using LibraryVault.Application.Interfaces.Services;

namespace LibraryVault.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<bool> ValidateCredentialsAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null) return false;

        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }

    public string GenerateToken(User user)
    {
        return _jwtTokenGenerator.GenerateToken(user);
    }
}