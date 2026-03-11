namespace TimeClockSystem.Core.DTOs.Shifts;

public class ClockOutResponseDto
{
    public int ShiftId { get; set; }
    public DateTimeOffset ClockInAt { get; set; }
    public DateTimeOffset ClockOutAt { get; set; }
    public string TimeSource { get; set; } = string.Empty;
}
