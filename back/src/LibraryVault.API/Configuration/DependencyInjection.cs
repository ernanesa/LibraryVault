using LibraryVault.Application.Interfaces;
using LibraryVault.Application.Interfaces.Repositories;
using LibraryVault.Application.Interfaces.Services;
using LibraryVault.Application.Services;
using LibraryVault.Infrastructure.Data;
using LibraryVault.Infrastructure.Repository;
using LibraryVault.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace LibraryVault.API.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<LibraryContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")
                  ?? "Data Source=LibraryVault.db"));

            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }
    }
}