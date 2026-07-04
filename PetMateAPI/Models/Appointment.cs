using PetMateAPI.Enums;

namespace PetMateAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;
public class Appointment
{
    public int Id { get; set; }

    // ── Vet ──────────────────────────────────────────────────────────
    public int VetId { get; set; }
    public string VetName { get; set; } = "";
    public string? VetImageUrl { get; set; }
    public string? ClinicName { get; set; }
    public string? ClinicAddress { get; set; }
    public double? ClinicLatitude { get; set; }
    public double? ClinicLongitude { get; set; }

    // ── User ─────────────────────────────────────────────────────────
    public string UserId { get; set; } = "";
    public string UserName { get; set; } = "";
    public string? UserPhone { get; set; }

    // ── Pet ───────────────────────────────────────────────────────────
    public int  PetId { get; set; } 
    public string PetName { get; set; } = "";
    public string? PetType { get; set; }
    public string? PetBreed { get; set; }

    // ── Appointment Details ───────────────────────────────────────────
    public DateTime AppointmentDate { get; set; }
    public string TimeSlot { get; set; } = "";
    public AppointmentType Type { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    // ── Service ───────────────────────────────────────────────────────
    public string? ServiceType { get; set; }
    public string? Notes { get; set; }

    // ── Home Visit ────────────────────────────────────────────────────
    public string? HomeAddress { get; set; }
    public double? HomeLatitude { get; set; }
    public double? HomeLongitude { get; set; }
    public string? HomeAddressNotes { get; set; }

    // ── Payment ───────────────────────────────────────────────────────
    public double ConsultationFee { get; set; }
    public double? HomeVisitFee { get; set; }
    public double? EmergencyFee { get; set; }
    public double TotalFee { get; set; }  // ← regular column

    [NotMapped]  
    public double CalculatedTotalFee =>
        ConsultationFee +
        (HomeVisitFee ?? 0) +
        (EmergencyFee ?? 0);

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public string? PaymentMethod { get; set; }

    // ── Timestamps ────────────────────────────────────────────────────
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
}
