namespace PetMateAPI.DTOs
{
    // ── Request DTOs (Client → Server) ────────────────────────────────────────

    public class BookAppointmentDto
    {
        public int VetId { get; set; }
        public string PetId { get; set; } = "";
        public DateTime AppointmentDate { get; set; }
        public string TimeSlot { get; set; } = "";
        public int Type { get; set; }            
        public string? ServiceType { get; set; }
        public string? Notes { get; set; }
        public string? PaymentMethod { get; set; }

        // ── Home visit ────────────────────────────────────────────────
        public string? HomeAddress { get; set; }
        public double? HomeLatitude { get; set; }
        public double? HomeLongitude { get; set; }
        public string? HomeAddressNotes { get; set; }
    }

    public class CancelAppointmentDto
    {
        public string Reason { get; set; } = "";
    }

    // ── Response DTOs (Server → Client) ──────────────────────────────────────

    public class AppointmentDto
    {
        public int Id { get; set; }

        // ── Vet ──────────────────────────────────────────────────────
        public int VetId { get; set; }
        public string VetName { get; set; } = "";
        public string? VetImageUrl { get; set; }
        public string? ClinicName { get; set; }
        public string? ClinicAddress { get; set; }
        public double? ClinicLatitude { get; set; }
        public double? ClinicLongitude { get; set; }

        // ── Pet ───────────────────────────────────────────────────────
        public string PetName { get; set; } = "";
        public string? PetType { get; set; }
        public string? PetBreed { get; set; }

        // ── Appointment ───────────────────────────────────────────────
        public DateTime AppointmentDate { get; set; }
        public string TimeSlot { get; set; } = "";
        public int Type { get; set; }
        public int Status { get; set; }
        public string? ServiceType { get; set; }
        public string? Notes { get; set; }

        // ── Home visit ────────────────────────────────────────────────
        public string? HomeAddress { get; set; }
        public double? HomeLatitude { get; set; }
        public double? HomeLongitude { get; set; }
        public string? HomeAddressNotes { get; set; }

        // ── Payment ───────────────────────────────────────────────────
        public double ConsultationFee { get; set; }
        public double? HomeVisitFee { get; set; }
        public double TotalFee { get; set; }
        public int PaymentStatus { get; set; }
        public string? PaymentMethod { get; set; }

        // ── Timestamps ────────────────────────────────────────────────
        public DateTime CreatedAt { get; set; }
        public string? CancellationReason { get; set; }
    }

    public class AvailableSlotsDto
    {
        public DateTime Date { get; set; }
        public int VetId { get; set; }
        public List<TimeSlotDto> Slots { get; set; } = new();
    }

    public class TimeSlotDto
    {
        public string Slot { get; set; } = "";       
        public bool IsAvailable { get; set; }
    }
}