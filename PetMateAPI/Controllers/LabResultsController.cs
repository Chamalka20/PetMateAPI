using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetMateAPI.DTOs;
using PetMateAPI.Services;
using System.Security.Claims;

namespace PetMateAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LabResultsController : ControllerBase
    {
        private readonly ILabResultService _labResultService;

        public LabResultsController(ILabResultService labResultService)
        {
            _labResultService = labResultService;
        }

        private string GetUserId() =>
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException();

        // ── Get my lab results ────────────────────────────────────────────
        [HttpGet("my")]
        public async Task<IActionResult> GetMyLabResults(
            [FromQuery] string? searchQuery = null,
            [FromQuery] int? type = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            try
            {
                var userId = GetUserId();
                var (results, total) = await _labResultService
                    .GetUserLabResults(userId, searchQuery, type, page, pageSize);

                return Ok(new { total, page, pageSize, data = results });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ── Get single ────────────────────────────────────────────────────
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLabResult(int id)
        {
            try
            {
                var result = await _labResultService.GetLabResultById(id);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ── Get by appointment ────────────────────────────────────────────
        [HttpGet("appointment/{appointmentId}")]
        public async Task<IActionResult> GetByAppointment(int appointmentId)
        {
            try
            {
                var results = await _labResultService
                    .GetByAppointmentId(appointmentId);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ── Create (vet/admin only) ───────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> CreateLabResult(
            [FromBody] CreateLabResultDto dto)
        {
            try
            {
                var result = await _labResultService.CreateLabResult(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
