using Microsoft.EntityFrameworkCore;
using PetMateAPI.Data;
using PetMateAPI.DTOs;
using PetMateAPI.Models;

namespace PetMateAPI.Services
{
    public interface IPrescriptionService
    {
        Task<List<PrescriptionDto>> GetUserPrescriptions(string userId);
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

        public async Task<List<PrescriptionDto>> GetUserPrescriptions(string userId)
        {
            var prescriptions = await _context.Prescriptions
                .Include(p => p.Medicines)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.IssuedAt)
                .ToListAsync();

            return prescriptions.Select(MapToDto).ToList();
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
