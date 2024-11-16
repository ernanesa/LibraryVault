using LibraryVault.Domain.Entities;

namespace LibraryVault.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}