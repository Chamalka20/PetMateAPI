using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetMateAPI.DTOs;
using PetMateAPI.Services;
using Supabase.Gotrue;
using System.Security.Claims;

namespace PetMateAPI.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]  
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }
    private string GetUserId() =>
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User not authenticated");
    // ── Book appointment ──────────────────────────────────────────────
    [HttpPost("book")]
    public async Task<IActionResult> BookAppointment(
           [FromBody] BookAppointmentDto dto)
    {
        try
        {
            var userId = GetUserId();
            var appointment = await _appointmentService
                .BookAppointment(userId, dto);
            return Ok(appointment);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // ── Get user upcoming appointments ─────────────────────────────────────────
    [HttpGet("my/upcoming")]
    public async Task<IActionResult> GetUpcomingAppointments()
    {
        try
        {
            var userId = GetUserId();
            var appointments = await _appointmentService
                .GetUpcomingAppointments(userId);
            return Ok(appointments);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    // ── Get user history appointments ─────────────────────────────────────────
    [HttpGet("my/history")]
    public async Task<IActionResult> GetAppointmentHistory()
    {
        try
        {
            var userId = GetUserId();
            var appointments = await _appointmentService
                .GetAppointmentHistory(userId);
            return Ok(appointments);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // ── Get appointment by id ─────────────────────────────────────────
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppointment(int id)
    {
        var appointment = await _appointmentService.GetAppointmentById(id);
        if (appointment == null) return NotFound();
        return Ok(appointment);
    }

    // ── Cancel appointment ────────────────────────────────────────────
    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> CancelAppointment(
        int id,
        [FromBody] CancelAppointmentDto dto)
    {
        try
        {
            var result = await _appointmentService
                .CancelAppointment(id, dto);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    // ── Get available time slots ──────────────────────────────────────
    [HttpGet("slots")]
    public async Task<IActionResult> GetAvailableSlots(
        [FromQuery] int vetId,
        [FromQuery] DateTime date)
    {
        var slots = await _appointmentService.GetAvailableSlots(vetId, date);
        return Ok(slots);
    }
}
