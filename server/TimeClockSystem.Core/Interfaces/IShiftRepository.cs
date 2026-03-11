using TimeClockSystem.Core.Entities;

namespace TimeClockSystem.Core.Interfaces;

public interface IShiftRepository
{
    Task<Shift?> GetOpenShiftByUserIdAsync(int userId);
    Task<Shift> CreateAsync(Shift shift);
    Task<Shift> UpdateAsync(Shift shift);
    Task<IEnumerable<Shift>> GetShiftHistoryByUserIdAsync(int userId);
}
