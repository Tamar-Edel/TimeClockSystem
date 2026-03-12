using Moq;
using TimeClockSystem.Core.Entities;
using TimeClockSystem.Core.Interfaces;
using TimeClockSystem.Core.Services;

namespace TimeClockSystem.Tests;

public class ShiftServiceTests
{
    private readonly Mock<IShiftRepository> _shiftRepoMock;
    private readonly Mock<IExternalTimeProvider> _timeProviderMock;
    private readonly ShiftService _service;

    public ShiftServiceTests()
    {
        _shiftRepoMock = new Mock<IShiftRepository>();
        _timeProviderMock = new Mock<IExternalTimeProvider>();
        _service = new ShiftService(_shiftRepoMock.Object, _timeProviderMock.Object);
    }

    // --- ClockIn ---

    // A user with no open shift can clock in and gets a response with the correct time source.
    [Fact]
    public async Task ClockIn_WhenNoOpenShift_ReturnsClockInResponse()
    {
        var now = DateTimeOffset.UtcNow;
        _shiftRepoMock.Setup(r => r.GetOpenShiftByUserIdAsync(1)).ReturnsAsync((Shift?)null);
        _timeProviderMock.Setup(t => t.GetCurrentTimeAsync()).ReturnsAsync(now);
        _shiftRepoMock.Setup(r => r.CreateAsync(It.IsAny<Shift>())).ReturnsAsync((Shift s) => s);

        var result = await _service.ClockInAsync(1);

        Assert.NotNull(result);
        Assert.Equal("timeapi.io", result.TimeSource);
        Assert.Equal(now, result.ClockInAt);
    }

    // A user who already has an open shift cannot clock in again.
    [Fact]
    public async Task ClockIn_WhenShiftAlreadyOpen_ThrowsInvalidOperationException()
    {
        _shiftRepoMock.Setup(r => r.GetOpenShiftByUserIdAsync(1)).ReturnsAsync(new Shift { Id = 1 });

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ClockInAsync(1));

        Assert.Equal("A shift is already open.", ex.Message);
    }

    // If the external time provider fails, the clock-in must fail too (no fallback).
    [Fact]
    public async Task ClockIn_WhenTimeProviderFails_ThrowsInvalidOperationException()
    {
        _shiftRepoMock.Setup(r => r.GetOpenShiftByUserIdAsync(1)).ReturnsAsync((Shift?)null);
        _timeProviderMock.Setup(t => t.GetCurrentTimeAsync())
            .ThrowsAsync(new InvalidOperationException("Unable to retrieve current time from external time provider."));

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ClockInAsync(1));
    }

    // --- ClockOut ---

    // A user with an open shift can clock out and gets a response with both times.
    [Fact]
    public async Task ClockOut_WhenOpenShiftExists_ReturnsClockOutResponse()
    {
        var clockIn = DateTimeOffset.UtcNow.AddHours(-2);
        var clockOut = DateTimeOffset.UtcNow;
        var shift = new Shift { Id = 7, UserId = 1, ClockInAt = clockIn, ClockInTimeSource = "timeapi.io" };

        _shiftRepoMock.Setup(r => r.GetOpenShiftByUserIdAsync(1)).ReturnsAsync(shift);
        _timeProviderMock.Setup(t => t.GetCurrentTimeAsync()).ReturnsAsync(clockOut);
        _shiftRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Shift>())).ReturnsAsync((Shift s) => s);

        var result = await _service.ClockOutAsync(1);

        Assert.NotNull(result);
        Assert.Equal(7, result.ShiftId);
        Assert.Equal(clockIn, result.ClockInAt);
        Assert.Equal(clockOut, result.ClockOutAt);
    }

    // A user with no open shift cannot clock out.
    [Fact]
    public async Task ClockOut_WhenNoOpenShift_ThrowsInvalidOperationException()
    {
        _shiftRepoMock.Setup(r => r.GetOpenShiftByUserIdAsync(1)).ReturnsAsync((Shift?)null);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ClockOutAsync(1));

        Assert.Equal("No open shift found.", ex.Message);
    }

    // --- GetCurrentShift ---

    // When the user has an open shift, GetCurrent returns a DTO with the correct data.
    [Fact]
    public async Task GetCurrentShift_WhenOpenShiftExists_ReturnsDto()
    {
        var clockIn = DateTimeOffset.UtcNow.AddHours(-1);
        var shift = new Shift { Id = 5, UserId = 1, ClockInAt = clockIn, ClockInTimeSource = "timeapi.io" };
        _shiftRepoMock.Setup(r => r.GetOpenShiftByUserIdAsync(1)).ReturnsAsync(shift);

        var result = await _service.GetCurrentShiftAsync(1);

        Assert.NotNull(result);
        Assert.Equal(5, result.ShiftId);
        Assert.Equal(clockIn, result.ClockInAt);
        Assert.Equal("timeapi.io", result.TimeSource);
    }

    // When the user has no open shift, GetCurrent returns null (no exception).
    [Fact]
    public async Task GetCurrentShift_WhenNoOpenShift_ReturnsNull()
    {
        _shiftRepoMock.Setup(r => r.GetOpenShiftByUserIdAsync(1)).ReturnsAsync((Shift?)null);

        var result = await _service.GetCurrentShiftAsync(1);

        Assert.Null(result);
    }
}
