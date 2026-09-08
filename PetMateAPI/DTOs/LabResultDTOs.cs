namespace PetMateAPI.DTOs
{
    public class LabResultDto
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public string PetName { get; set; } = "";
        public string VetName { get; set; } = "";
        public string? VetImageUrl { get; set; }
        public string? ClinicAddress { get; set; }
        public int Type { get; set; }
        public string TestName { get; set; } = "";
        public string? Notes { get; set; }
        public string? PdfUrl { get; set; }
        public int Status { get; set; }
        public DateTime TestedAt { get; set; }
        public List<LabResultItemDto> Items { get; set; } = new();
    }

    public class LabResultItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
        public string Unit { get; set; } = "";
        public string? NormalRange { get; set; }
        public bool IsAbnormal { get; set; }
    }

    public class CreateLabResultDto
    {
        public int AppointmentId { get; set; }
        public int Type { get; set; }
        public string TestName { get; set; } = "";
        public string? Notes { get; set; }
        public string? PdfUrl { get; set; }
        public int Status { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public List<CreateLabResultItemDto> Items { get; set; } = new();
    }

    public class CreateLabResultItemDto
    {
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
        public string Unit { get; set; } = "";
        public string? NormalRange { get; set; }
        public bool IsAbnormal { get; set; }
    }
}
