using TimeClockSystem.Core.DTOs.Shifts;
using TimeClockSystem.Core.Entities;
using TimeClockSystem.Core.Interfaces;

namespace TimeClockSystem.Core.Services;

public class ShiftService
{
    private readonly IShiftRepository _shiftRepository;
    private readonly IExternalTimeProvider _timeProvider;

    public ShiftService(IShiftRepository shiftRepository, IExternalTimeProvider timeProvider)
    {
        _shiftRepository = shiftRepository;
        _timeProvider = timeProvider;
    }

    public async Task<ClockInResponseDto> ClockInAsync(int userId)
    {
        var openShift = await _shiftRepository.GetOpenShiftByUserIdAsync(userId);
        if (openShift is not null)
            throw new InvalidOperationException("A shift is already open.");

        var currentTime = await _timeProvider.GetCurrentTimeAsync();

        var shift = new Shift
        {
            UserId = userId,
            ClockInAt = currentTime,
            ClockInTimeSource = "timeapi.io",
            CreatedAtUtc = DateTime.UtcNow
        };

        await _shiftRepository.CreateAsync(shift);

        return new ClockInResponseDto
        {
            ShiftId = shift.Id,
            ClockInAt = shift.ClockInAt,
            TimeSource = shift.ClockInTimeSource
        };
    }

    public async Task<ClockOutResponseDto> ClockOutAsync(int userId)
    {
        var shift = await _shiftRepository.GetOpenShiftByUserIdAsync(userId);
        if (shift is null)
            throw new InvalidOperationException("No open shift found.");

        var currentTime = await _timeProvider.GetCurrentTimeAsync();

        if (currentTime < shift.ClockInAt)
            throw new InvalidOperationException("Clock out time cannot be before clock in time.");

        shift.ClockOutAt = currentTime;
        shift.ClockOutTimeSource = "timeapi.io";

        await _shiftRepository.UpdateAsync(shift);

        return new ClockOutResponseDto
        {
            ShiftId = shift.Id,
            ClockInAt = shift.ClockInAt,
            ClockOutAt = shift.ClockOutAt.Value,
            TimeSource = shift.ClockOutTimeSource
        };
    }

    public async Task<CurrentShiftDto?> GetCurrentShiftAsync(int userId)
    {
        var shift = await _shiftRepository.GetOpenShiftByUserIdAsync(userId);
        if (shift is null)
            return null;

        return new CurrentShiftDto
        {
            ShiftId = shift.Id,
            ClockInAt = shift.ClockInAt,
            TimeSource = shift.ClockInTimeSource
        };
    }

    public async Task<IEnumerable<ShiftHistoryItemDto>> GetShiftHistoryAsync(int userId)
    {
        var shifts = await _shiftRepository.GetShiftHistoryByUserIdAsync(userId);

        return shifts.Select(shift => new ShiftHistoryItemDto
        {
            ShiftId = shift.Id,
            ClockInAt = shift.ClockInAt,
            ClockOutAt = shift.ClockOutAt,
            DurationMinutes = shift.ClockOutAt.HasValue
                ? (shift.ClockOutAt.Value - shift.ClockInAt).TotalMinutes
                : null,
            IsOpen = shift.ClockOutAt == null
        });
    }
}
