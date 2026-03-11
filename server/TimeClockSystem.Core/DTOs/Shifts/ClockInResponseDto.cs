namespace TimeClockSystem.Core.DTOs.Shifts;

public class ClockInResponseDto
{
    public int ShiftId { get; set; }
    public DateTimeOffset ClockInAt { get; set; }
    public string TimeSource { get; set; } = string.Empty;
}
