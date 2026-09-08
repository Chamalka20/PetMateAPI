using PetMateAPI.Enums;

namespace PetMateAPI.Models
{
    public class LabResult
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }

        // ── User ─────────────────────────────────────────────────────
        public string UserId { get; set; } = "";

        // ── Pet ───────────────────────────────────────────────────────
        public string PetId { get; set; } = "";
        public string PetName { get; set; } = "";

        // ── Vet ───────────────────────────────────────────────────────
        public int VetId { get; set; }
        public string VetName { get; set; } = "";
        public string? VetImageUrl { get; set; }
        public string? ClinicAddress { get; set; }

        // ── Lab Result Details ────────────────────────────────────────
        public LabResultType Type { get; set; }
        public string TestName { get; set; } = "";
        public string? Notes { get; set; }
        public string? PdfUrl { get; set; }
        public LabResultStatus Status { get; set; } = LabResultStatus.Pending;

        // ── Timestamps ────────────────────────────────────────────────
        public DateTime TestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }

        // ── Items ─────────────────────────────────────────────────────
        public List<LabResultItem> Items { get; set; } = new();
    }

    public class LabResultItem
    {
        public int Id { get; set; }
        public int LabResultId { get; set; }
        public string Name { get; set; } = "";   // "RBC"
        public string Value { get; set; } = "";   // "4.5"
        public string Unit { get; set; } = "";   // "million/μL"
        public string? NormalRange { get; set; }         // "4.0-5.5"
        public bool IsAbnormal { get; set; }

        // ── Navigation property ───────────────────────────────────────
        public LabResult? LabResult { get; set; }
    }

}
