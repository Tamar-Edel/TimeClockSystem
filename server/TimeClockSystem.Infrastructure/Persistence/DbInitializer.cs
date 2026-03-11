using Microsoft.EntityFrameworkCore;
using TimeClockSystem.Core.Entities;
using TimeClockSystem.Core.Interfaces;

namespace TimeClockSystem.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context, IPasswordHasher passwordHasher)
    {
        if (await context.Users.AnyAsync()) return;

        var user = new User
        {
            FullName = "Demo User",
            Email = "demo@timeclock.com",
            PasswordHash = passwordHasher.Hash("Demo1234!"),
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();
    }
}
