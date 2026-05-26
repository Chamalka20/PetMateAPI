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
    [HttpPost]
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

    // ── Get user appointments ─────────────────────────────────────────
    [HttpGet("my")]
    public async Task<IActionResult> GetMyAppointments()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException();

        var appointments = await _appointmentService.GetUserAppointments(userId);
        return Ok(appointments);
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

            if (!result)
                return NotFound($"Appointment {id} not found");

            return Ok("Appointment cancelled successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
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
