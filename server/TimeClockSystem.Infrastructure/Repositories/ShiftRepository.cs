using Microsoft.EntityFrameworkCore;
using TimeClockSystem.Core.Entities;
using TimeClockSystem.Core.Interfaces;
using TimeClockSystem.Infrastructure.Persistence;

namespace TimeClockSystem.Infrastructure.Repositories;

public class ShiftRepository : IShiftRepository
{
    private readonly AppDbContext _context;

    public ShiftRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Shift?> GetOpenShiftByUserIdAsync(int userId)
    {
        return await _context.Shifts
            .FirstOrDefaultAsync(s => s.UserId == userId && s.ClockOutAt == null);
    }

    public async Task<Shift> CreateAsync(Shift shift)
    {
        _context.Shifts.Add(shift);
        await _context.SaveChangesAsync();
        return shift;
    }

    public async Task<Shift> UpdateAsync(Shift shift)
    {
        _context.Shifts.Update(shift);
        await _context.SaveChangesAsync();
        return shift;
    }

    public async Task<IEnumerable<Shift>> GetShiftHistoryByUserIdAsync(int userId)
    {
        return await _context.Shifts
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.ClockInAt)
            .ToListAsync();
    }
}
