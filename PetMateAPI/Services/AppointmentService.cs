using Microsoft.EntityFrameworkCore;
using PetMateAPI.Data;
using PetMateAPI.DTOs;
using PetMateAPI.Enums;
using PetMateAPI.Models;

namespace PetMateAPI.Services;

    public interface IAppointmentService
    {
        Task<AppointmentDto> BookAppointment(string userId, BookAppointmentDto dto);
        Task<List<AppointmentDto>> GetUpcomingAppointments(string userId); 
        Task<List<AppointmentDto>> GetAppointmentHistory(string userId);
        Task<AppointmentDto?> GetAppointmentById(int id);
        Task<object> CancelAppointment(int id, CancelAppointmentDto dto);
        Task<AvailableSlotsDto> GetAvailableSlots(int vetId, DateTime date);
    }


public class AppointmentService : IAppointmentService
{
    private readonly AppDbContext _context;

    public AppointmentService(AppDbContext context)
    {
        _context = context;
    }

    // ── Book Appointment ──────────────────────────────────────────────────
    public async Task<AppointmentDto> BookAppointment(
        string userId,
        BookAppointmentDto dto)
    {
        // ── Validate vet exists ───────────────────────────────────────────
        var vet = await _context.Vets.FindAsync(dto.VetId)
            ?? throw new Exception("Vet not found");

        // ── Validate home visit has address ───────────────────────────────
        if (dto.Type == (int)AppointmentType.HomeVisit &&
            string.IsNullOrEmpty(dto.HomeAddress))
        {
            throw new ArgumentException(
                "Home address is required for home visits"
            );
        }

        // ── Check slot availability ───────────────────────────────────────
        var isSlotTaken = await _context.Appointments.AnyAsync(a =>
            a.VetId == dto.VetId &&
            a.AppointmentDate == dto.AppointmentDate &&
            a.TimeSlot == dto.TimeSlot &&
            a.Status != AppointmentStatus.CancelledByAdmin &&
            a.Status != AppointmentStatus.CancelledByVet
        );

        if (isSlotTaken)
        {
            throw new Exception("This time slot is already booked");
        }

        // ── Calculate fees ────────────────────────────────────────────────
        var consultationFee = vet.Price ?? 0;
        double? homeVisitFee = dto.Type == (int)AppointmentType.HomeVisit
            ? 500
            : null;
        double? emergencyFee = dto.Type == (int)AppointmentType.Emergency
            ? 1000
            : null;

        // ── Create appointment ────────────────────────────────────────────
        var appointment = new Appointment
        {
            VetId = dto.VetId,
            VetName = vet.Name,
            VetImageUrl = vet.ImageUrl,
            ClinicName = vet.ClinicName,
            ClinicAddress = vet.Location,
            ClinicLatitude = vet.Latitude,
            ClinicLongitude = vet.Longitude,
            UserId = userId,
            AppointmentDate = dto.AppointmentDate,
            TimeSlot = dto.TimeSlot,
            Type = (AppointmentType)dto.Type,
            Status = AppointmentStatus.Pending,
            ServiceType = dto.ServiceType,
            Notes = dto.Notes,
            PaymentMethod = dto.PaymentMethod,
            ConsultationFee = consultationFee,
            HomeVisitFee = homeVisitFee,
            HomeAddress = dto.HomeAddress,
            HomeLatitude = dto.HomeLatitude,
            HomeLongitude = dto.HomeLongitude,
            HomeAddressNotes = dto.HomeAddressNotes,
            PaymentStatus = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        return MapToDto(appointment);
    }

    // ── Get User Upcoming Appointments ─────────────────────────────────────────────
    public async Task<List<AppointmentDto>> GetUpcomingAppointments(string userId)
    {
        var now = DateTime.UtcNow;

        var appointments = await _context.Appointments
            .Where(a =>
                a.UserId == userId &&
                a.AppointmentDate >= now &&           
                a.Status != AppointmentStatus.CancelledByUser &&
                a.Status != AppointmentStatus.CancelledByVet &&
                a.Status != AppointmentStatus.CancelledByAdmin &&
                a.Status != AppointmentStatus.Completed       
            )
            .OrderBy(a => a.AppointmentDate)             
            .ToListAsync();

        return appointments.Select(MapToDto).ToList();
    }

    // ── Get User Appointments History ─────────────────────────────────────────────
    public async Task<List<AppointmentDto>> GetAppointmentHistory(string userId)
    {
        var now = DateTime.UtcNow;

        var appointments = await _context.Appointments
            .Where(a =>
                a.UserId == userId &&
                (
                    a.AppointmentDate < now ||                             
                    a.Status == AppointmentStatus.Completed ||      
                    a.Status == AppointmentStatus.CancelledByUser ||     
                    a.Status == AppointmentStatus.CancelledByVet ||
                    a.Status == AppointmentStatus.CancelledByAdmin
                )
            )
            .OrderByDescending(a => a.AppointmentDate)   
            .ToListAsync();

        return appointments.Select(MapToDto).ToList();
    }

    // ── Get Appointment By Id ─────────────────────────────────────────────
    public async Task<AppointmentDto?> GetAppointmentById(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        return appointment == null ? null : MapToDto(appointment);
    }

    // ── Cancel Appointment ────────────────────────────────────────────────
    public async Task<object> CancelAppointment(int id, CancelAppointmentDto dto)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null) return false;

        // ── Can't cancel completed appointments ───────────────────────────
        if (appointment.Status == AppointmentStatus.Completed)
        {
            throw new Exception("Cannot cancel a completed appointment");
        }

        // ── Set status based on who cancelled ────────────────────────────
        appointment.Status = dto.CancelledBy.ToLower() switch
        {
            "vet" => AppointmentStatus.CancelledByVet,
            "admin" => AppointmentStatus.CancelledByAdmin,
            _ => AppointmentStatus.CancelledByUser   
        };

        appointment.CancelledAt = DateTime.UtcNow;
        appointment.CancellationReason = dto.Reason;
        appointment.UpdatedAt = DateTime.UtcNow;

        // ── Refund if already paid ────────────────────────────────────────
        if (appointment.PaymentStatus == PaymentStatus.Paid)
        {
            appointment.PaymentStatus = PaymentStatus.Refunded;
        }

        await _context.SaveChangesAsync();
        return new { message = "Appointment cancelled successfully" };
    }

    // ── Get Available Slots ───────────────────────────────────────────────
    public async Task<AvailableSlotsDto> GetAvailableSlots(
        int vetId,
        DateTime date)
    {
        // ── All possible slots ────────────────────────────────────────────
        var allSlots = new List<string>
{
    "08:00 AM",
    "08:30 AM",
    "09:00 AM",
    "09:30 AM",
    "10:00 AM",
    "10:30 AM",
    "11:00 AM",
    "11:30 AM",
    "12:00 PM",
    "12:30 PM",
    "01:00 PM",
    "01:30 PM",
    "02:00 PM",
    "02:30 PM",
    "03:00 PM",
    "03:30 PM",
    "04:00 PM",
    "04:30 PM",
    "05:00 PM",
    "05:30 PM",
    "06:00 PM",
    "06:30 PM"
};

        // ── Get already booked slots ──────────────────────────────────────
        var bookedSlots = await _context.Appointments
            .Where(a =>
                a.VetId == vetId &&
                a.AppointmentDate == date &&
                a.Status != AppointmentStatus.CancelledByAdmin&&
                a.Status != AppointmentStatus.CancelledByVet
            )
            .Select(a => a.TimeSlot)
            .ToListAsync();

        // ── Map slots with availability ───────────────────────────────────
        var slots = allSlots.Select(slot => new TimeSlotDto
        {
            Slot = slot,
            IsAvailable = !bookedSlots.Contains(slot)
        }).ToList();

        return new AvailableSlotsDto
        {
            Date = date,
            VetId = vetId,
            Slots = slots
        };
    }

    // ── Map Model → DTO ───────────────────────────────────────────────────
    private AppointmentDto MapToDto(Appointment appointment)
    {
        var totalFee = appointment.ConsultationFee
                     + (appointment.HomeVisitFee ?? 0)
                    ;

        return new AppointmentDto
        {
            Id = appointment.Id,
            VetId = appointment.VetId,
            VetName = appointment.VetName,
            VetImageUrl = appointment.VetImageUrl,
            ClinicName = appointment.ClinicName,
            ClinicAddress = appointment.ClinicAddress,
            ClinicLatitude = appointment.ClinicLatitude,
            ClinicLongitude = appointment.ClinicLongitude,
            PetName = appointment.PetName,
            PetType = appointment.PetType,
            PetBreed = appointment.PetBreed,
            AppointmentDate = appointment.AppointmentDate,
            TimeSlot = appointment.TimeSlot,
            Type = (int)appointment.Type,
            Status = (int)appointment.Status,
            ServiceType = appointment.ServiceType,
            Notes = appointment.Notes,
            HomeAddress = appointment.HomeAddress,
            HomeLatitude = appointment.HomeLatitude,
            HomeLongitude = appointment.HomeLongitude,
            HomeAddressNotes = appointment.HomeAddressNotes,
            ConsultationFee = appointment.ConsultationFee,
            HomeVisitFee = appointment.HomeVisitFee,
            TotalFee = totalFee,
            PaymentStatus = (int)appointment.PaymentStatus,
            PaymentMethod = appointment.PaymentMethod,
            CreatedAt = appointment.CreatedAt,
            CancellationReason = appointment.CancellationReason
        };
    }
}