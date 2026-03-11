using TimeClockSystem.Core.DTOs.Auth;
using TimeClockSystem.Core.Entities;
using TimeClockSystem.Core.Interfaces;

namespace TimeClockSystem.Core.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<LoginResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        var existing = await _userRepository.GetByEmailAsync(dto.Email);
        if (existing is not null)
            throw new InvalidOperationException("Email is already registered.");

        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = _passwordHasher.Hash(dto.Password),
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(user);

        var token = _tokenService.GenerateToken(user);

        return new LoginResponseDto
        {
            Token = token,
            FullName = user.FullName,
            Email = user.Email
        };
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user is null)
            throw new InvalidOperationException("Invalid email or password.");

        if (!user.IsActive)
            throw new InvalidOperationException("Account is inactive.");

        if (!_passwordHasher.Verify(dto.Password, user.PasswordHash))
            throw new InvalidOperationException("Invalid email or password.");

        var token = _tokenService.GenerateToken(user);

        return new LoginResponseDto
        {
            Token = token,
            FullName = user.FullName,
            Email = user.Email
        };
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id);
    }
}
