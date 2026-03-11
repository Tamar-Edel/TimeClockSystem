namespace TimeClockSystem.Core.Entities;

public class Shift
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTimeOffset ClockInAt { get; set; }
    public DateTimeOffset? ClockOutAt { get; set; }
    public string ClockInTimeSource { get; set; } = string.Empty;
    public string? ClockOutTimeSource { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public User User { get; set; } = null!;
}
