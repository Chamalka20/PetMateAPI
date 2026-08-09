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
    public class PrescriptionsController : ControllerBase
    {
        private readonly IPrescriptionService _prescriptionService;

        public PrescriptionsController(IPrescriptionService prescriptionService)
        {
            _prescriptionService = prescriptionService;
        }

        private string GetUserId() =>
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException();

        // ── Get my prescriptions ──────────────────────────────────────────
        [HttpGet("my")]
        public async Task<IActionResult> GetMyPrescriptions([FromQuery] string? searchQuery = null,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetUserId();
                var (prescriptions, total) = await _prescriptionService
                    .GetUserPrescriptions(userId, searchQuery, page, pageSize);

                return Ok(new
                {
                    total,
                    page,
                    pageSize,
                    data = prescriptions
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ── Get single prescription ───────────────────────────────────────
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrescription(int id)
        {
            try
            {
                var prescription = await _prescriptionService
                    .GetPrescriptionById(id);
                if (prescription == null) return NotFound();
                return Ok(prescription);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ── Create prescription (vet/admin only) ──────────────────────────
        [HttpPost]
        [HttpPost("create")]
        public async Task<IActionResult> CreatePrescription(
            [FromBody] CreatePrescriptionDto dto)
        {
            try
            {
                var prescription = await _prescriptionService
                    .CreatePrescription(dto);
                return Ok(prescription);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ── Get prescriptions by appointment ──────────────────────────────
        [HttpGet("appointment/{appointmentId}")]
        public async Task<IActionResult> GetByAppointment(int appointmentId)
        {
            try
            {
                var prescriptions = await _prescriptionService
                    .GetByAppointmentId(appointmentId);
                return Ok(prescriptions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
