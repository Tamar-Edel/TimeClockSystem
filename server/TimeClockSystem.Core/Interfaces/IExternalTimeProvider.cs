namespace TimeClockSystem.Core.Interfaces;

public interface IExternalTimeProvider
{
    Task<DateTimeOffset> GetCurrentTimeAsync();
}
