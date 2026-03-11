using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeClockSystem.Core.Services;

namespace TimeClockSystem.API.Controllers;

[ApiController]
[Route("api/shifts")]
[Authorize]
public class ShiftsController : ControllerBase
{
    private readonly ShiftService _shiftService;

    public ShiftsController(ShiftService shiftService)
    {
        _shiftService = shiftService;
    }

    private int GetUserId()
    {
        return int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent()
    {
        var shift = await _shiftService.GetCurrentShiftAsync(GetUserId());

        if (shift is null)
            return Ok(new { message = "No open shift." });

        return Ok(shift);
    }

    [HttpPost("clock-in")]
    public async Task<IActionResult> ClockIn()
    {
        try
        {
            var result = await _shiftService.ClockInAsync(GetUserId());
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("clock-out")]
    public async Task<IActionResult> ClockOut()
    {
        try
        {
            var result = await _shiftService.ClockOutAsync(GetUserId());
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory()
    {
        var history = await _shiftService.GetShiftHistoryAsync(GetUserId());
        return Ok(history);
    }
}
