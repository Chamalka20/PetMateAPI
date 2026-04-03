namespace PetMateAPI.DTOs
{
    public class CreatePetDto
    {
        public string Name { get; set; }
        public string? Type { get; set; }
        public string Breed { get; set; }
        public int Age { get; set; }
        public double Weight { get; set; }
        public string Gender { get; set; }
        public bool IsSpayedNeutered { get; set; }
        public List<string> MedicalConditions { get; set; }
        public List<string> Allergies { get; set; }
        public string ImageUrl { get; set; }
    }
}
