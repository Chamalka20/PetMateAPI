namespace PetMateAPI.Models
{
    public class Prescription
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public string UserId { get; set; } = "";
        public string PetId { get; set; } = "";
        public string PetName { get; set; } = "";
        public int VetId { get; set; }
        public string VetName { get; set; } = "";
        public string? VetImageUrl { get; set; }
        public string? ClinicAddress { get; set; }
        public int AppointmentType { get; set; }
        public string? Diagnosis { get; set; }
        public string? Notes { get; set; }
        public string? PdfUrl { get; set; }  
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }
        public List<Medicine> Medicines { get; set; } = new();
    }

    public class Medicine
    {
        public int Id { get; set; }
        public int PrescriptionId { get; set; }
        public string Name { get; set; } = "";
        public string Dosage { get; set; } = "";
        public string Frequency { get; set; } = "";
        public int DurationDays { get; set; }
        public string? Instructions { get; set; }
        public string? Form { get; set; }
    }
}
