using Microsoft.EntityFrameworkCore;
using PetMateAPI.Data;
using PetMateAPI.DTOs;
using PetMateAPI.Enums;
using PetMateAPI.Models;

namespace PetMateAPI.Services
{
    public interface ILabResultService
    {
        Task<(List<LabResultDto> Results, int Total)> GetUserLabResults(
            string userId, string? searchQuery, int? type, int page, int pageSize);
        Task<LabResultDto?> GetLabResultById(int id);
        Task<List<LabResultDto>> GetByAppointmentId(int appointmentId);
        Task<LabResultDto> CreateLabResult(CreateLabResultDto dto);
    }

    public class LabResultService : ILabResultService
    {
        private readonly AppDbContext _context;

        public LabResultService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<LabResultDto> Results, int Total)> GetUserLabResults(
            string userId,
            string? searchQuery,
            int? type,
            int page,
            int pageSize)
        {
            var query = _context.LabResults
                .Include(l => l.Items)
                .Where(l => l.UserId == userId);

            // ── Search ────────────────────────────────────────────────────
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(l =>
                    l.TestName.Contains(searchQuery) ||
                    l.VetName.Contains(searchQuery) ||
                    l.PetName.Contains(searchQuery)
                );
            }

            // ── Filter by type ────────────────────────────────────────────
            if (type.HasValue)
            {
                query = query.Where(l => (int)l.Type == type.Value);
            }

            var total = await query.CountAsync();
            var results = await query
                .OrderByDescending(l => l.TestedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (results.Select(MapToDto).ToList(), total);
        }

        public async Task<LabResultDto?> GetLabResultById(int id)
        {
            var result = await _context.LabResults
                .Include(l => l.Items)
                .FirstOrDefaultAsync(l => l.Id == id);

            return result == null ? null : MapToDto(result);
        }

        public async Task<List<LabResultDto>> GetByAppointmentId(int appointmentId)
        {
            var results = await _context.LabResults
                .Include(l => l.Items)
                .Where(l => l.AppointmentId == appointmentId)
                .ToListAsync();

            return results.Select(MapToDto).ToList();
        }

        public async Task<LabResultDto> CreateLabResult(CreateLabResultDto dto)
        {
            var appointment = await _context.Appointments
                .FindAsync(dto.AppointmentId)
                ?? throw new Exception("Appointment not found");

            var labResult = new LabResult
            {
                AppointmentId = dto.AppointmentId,
                UserId = appointment.UserId,
                PetId = appointment.PetId.ToString(),
                PetName = appointment.PetName,
                VetId = appointment.VetId,
                VetName = appointment.VetName,
                VetImageUrl = appointment.VetImageUrl,
                ClinicAddress = appointment.ClinicAddress,
                Type = (LabResultType)dto.Type,
                TestName = dto.TestName,
                Notes = dto.Notes,
                PdfUrl = dto.PdfUrl,
                Status = (LabResultStatus)dto.Status,
                ExpiresAt = dto.ExpiresAt,
                TestedAt = DateTime.UtcNow,
                Items = dto.Items.Select(i => new LabResultItem
                {
                    Name = i.Name,
                    Value = i.Value,
                    Unit = i.Unit,
                    NormalRange = i.NormalRange,
                    IsAbnormal = i.IsAbnormal
                }).ToList()
            };

            _context.LabResults.Add(labResult);
            await _context.SaveChangesAsync();

            return MapToDto(labResult);
        }

        private LabResultDto MapToDto(LabResult l) => new()
        {
            Id = l.Id,
            AppointmentId = l.AppointmentId,
            PetName = l.PetName,
            VetName = l.VetName,
            VetImageUrl = l.VetImageUrl,
            ClinicAddress = l.ClinicAddress,
            Type = (int)l.Type,
            TestName = l.TestName,
            Notes = l.Notes,
            PdfUrl = l.PdfUrl,
            Status = (int)l.Status,
            TestedAt = l.TestedAt,
            Items = l.Items.Select(i => new LabResultItemDto
            {
                Id = i.Id,
                Name = i.Name,
                Value = i.Value,
                Unit = i.Unit,
                NormalRange = i.NormalRange,
                IsAbnormal = i.IsAbnormal
            }).ToList()
        };
    }
}
