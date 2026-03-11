using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TimeClockSystem.Core.Interfaces;
using TimeClockSystem.Infrastructure.ExternalServices;
using TimeClockSystem.Infrastructure.Persistence;
using TimeClockSystem.Infrastructure.Repositories;
using TimeClockSystem.Infrastructure.Security;

namespace TimeClockSystem.Infrastructure.DependencyInjection;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IShiftRepository, ShiftRepository>();
        services.AddScoped<IExternalTimeProvider, ExternalTimeProvider>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

        services.AddHttpClient();

        return services;
    }
}
