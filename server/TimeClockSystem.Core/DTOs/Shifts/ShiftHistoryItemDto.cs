namespace TimeClockSystem.Core.DTOs.Shifts;

public class ShiftHistoryItemDto
{
    public int ShiftId { get; set; }
    public DateTimeOffset ClockInAt { get; set; }
    public DateTimeOffset? ClockOutAt { get; set; }
    public double? DurationMinutes { get; set; }
    public bool IsOpen { get; set; }
}
