using Microsoft.EntityFrameworkCore;
using PetMateAPI.Data;
using PetMateAPI.DTOs;
using PetMateAPI.Models;

namespace PetMateAPI.Services
{
    public interface IPrescriptionService
    {
        Task<(List<PrescriptionDto> Prescriptions, int Total)> GetUserPrescriptions(
            string userId,
            string? searchQuery = null,
            int page = 1,
             int pageSize = 10
           );
        Task<PrescriptionDto?> GetPrescriptionById(int id);
        Task<PrescriptionDto> CreatePrescription(CreatePrescriptionDto dto);
        Task<List<PrescriptionDto>> GetByAppointmentId(int appointmentId);
    }

    public class PrescriptionService : IPrescriptionService
    {
        private readonly AppDbContext _context;

        public PrescriptionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<PrescriptionDto> Prescriptions, int Total)> GetUserPrescriptions(string userId,
     string? searchQuery = null,
     int page = 1,
     int pageSize = 10
    )
        {

            var query = _context.Prescriptions
                .Include(p => p.Medicines)
                .Where(p => p.UserId == userId);

            // ── Search filter ─────────────────────────────────────────────────
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(p =>
                    p.VetName.Contains(searchQuery) ||
                    p.PetName.Contains(searchQuery) ||
                    (p.Diagnosis != null && p.Diagnosis.Contains(searchQuery))
                );
            }

            var total = await query.CountAsync();
            var prescriptions = await query
                .OrderByDescending(p => p.IssuedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (prescriptions.Select(MapToDto).ToList(), total);
        }

        public async Task<PrescriptionDto?> GetPrescriptionById(int id)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.Medicines)
                .FirstOrDefaultAsync(p => p.Id == id);

            return prescription == null ? null : MapToDto(prescription);
        }

        public async Task<PrescriptionDto> CreatePrescription(
            CreatePrescriptionDto dto)
        {
            // ── Get appointment details ───────────────────────────────────
            var appointment = await _context.Appointments
                .FindAsync(dto.AppointmentId)
                ?? throw new Exception("Appointment not found");

            var prescription = new Prescription
            {
                AppointmentId = dto.AppointmentId,
                UserId = appointment.UserId,
                PetId = appointment.PetId.ToString(),
                PetName = appointment.PetName,
                VetId = appointment.VetId,
                VetName = appointment.VetName,
                VetImageUrl = appointment.VetImageUrl,
                ClinicAddress = appointment.ClinicAddress,
                AppointmentType = (int)appointment.Type,
                Diagnosis = dto.Diagnosis,
                Notes = dto.Notes,
                PdfUrl = dto.PdfUrl,
                ExpiresAt = dto.ExpiresAt,
                IssuedAt = DateTime.UtcNow,
                Medicines = dto.Medicines.Select(m => new Medicine
                {
                    Name = m.Name,
                    Dosage = m.Dosage,
                    Frequency = m.Frequency,
                    DurationDays = m.DurationDays,
                    Instructions = m.Instructions,
                    Form = m.Form
                }).ToList()
            };

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            return MapToDto(prescription);
        }

        public async Task<List<PrescriptionDto>> GetByAppointmentId(
            int appointmentId)
        {
            var prescriptions = await _context.Prescriptions
                .Include(p => p.Medicines)
                .Where(p => p.AppointmentId == appointmentId)
                .ToListAsync();

            return prescriptions.Select(MapToDto).ToList();
        }

        private PrescriptionDto MapToDto(Prescription p) => new()
        {
            Id = p.Id,
            AppointmentId = p.AppointmentId,
            PetName = p.PetName,
            VetName = p.VetName,
            VetImageUrl = p.VetImageUrl,
            ClinicAddress = p.ClinicAddress,
            AppointmentType = p.AppointmentType,
            Diagnosis = p.Diagnosis,
            Notes = p.Notes,
            PdfUrl = p.PdfUrl,
            IssuedAt = p.IssuedAt,
            Medicines = p.Medicines.Select(m => new MedicineDto
            {
                Id = m.Id,
                Name = m.Name,
                Dosage = m.Dosage,
                Frequency = m.Frequency,
                DurationDays = m.DurationDays,
                Instructions = m.Instructions,
                Form = m.Form
            }).ToList()
        };
    }
}
