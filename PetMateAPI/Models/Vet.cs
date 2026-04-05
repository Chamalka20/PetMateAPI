namespace PetMateAPI.Models
{
    public class Vet
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? ClinicName { get; set; }
        public string? Location { get; set; }
        public double Rating { get; set; }
        public List<string>? AvailableDays { get; set; }
        public List<string>? Specializations { get; set; }
        public string? ImageUrl { get; set; }
    }
}
