using TimeClockSystem.Core.Entities;

namespace TimeClockSystem.Core.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}
