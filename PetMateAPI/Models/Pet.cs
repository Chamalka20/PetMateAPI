namespace PetMateAPI.Models
{
    public class Pet
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Type { get; set; }
        public string Breed { get; set; } = string.Empty;
        public int Age { get; set; }
        public double Weight { get; set; }
        public string Gender { get; set; } = "Male";
        public bool IsSpayedNeutered { get; set; }
        public string MedicalConditions { get; set; } = "";
        public string Allergies { get; set; } = "";
        public string ImageUrl { get; set; } = "";

        // optional: link to user
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
