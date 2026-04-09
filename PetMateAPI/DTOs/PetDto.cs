namespace PetMateAPI.DTOs
{
    public class PetDto
    {
        public string Name { get; set; }
        public string? Type { get; set; }
        public string Breed { get; set; }
        public int Age { get; set; }
        public double Weight { get; set; }
        public string Gender { get; set; }
        public bool IsSpayedNeutered { get; set; }
        public List<string?>? MedicalConditions { get; set; }
        public List<string?> ? Allergies { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class PetResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Type { get; set; }
        public string Breed { get; set; } = string.Empty;
        public int Age { get; set; }
        public double Weight { get; set; }
        public string Gender { get; set; } = "Male";
        public bool IsSpayedNeutered { get; set; }
        public List<string> MedicalConditions { get; set; } = new();
        public List<string> Allergies { get; set; } = new();
        public string ImageUrl { get; set; } = "";

        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class UpdatePetDto
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Breed { get; set; }
        public int? Age { get; set; }
        public double? Weight { get; set; }
        public string? Gender { get; set; }
        public bool? IsSpayedNeutered { get; set; }
        public List<string?>? MedicalConditions { get; set; }
        public List<string?>? Allergies { get; set; }
        public string? ImageUrl { get; set; }
    }
}
