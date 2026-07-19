namespace PetMateAPI.DTOs
{
    public class VetDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? ClinicName { get; set; }
        public string? Location { get; set; }
        public double Rating { get; set; }

        public List<string> Services { get; set; } = new();

        public int? ExperienceYears { get; set; }
        public double? Price { get; set; }
        public string? WorkingDays { get; set; }
        public string? WorkingTime { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? RewardPoints { get; set; }
        public int? WaitingTimeMinutes { get; set; }
        public string? ImageUrl { get; set; }
    }
}
