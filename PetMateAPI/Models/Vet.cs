namespace PetMateAPI.Models
{
    public class Vet
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? ClinicName { get; set; }       
        public string? Location { get; set; }
        public double Rating { get; set; }
        public List<string>? Specializations { get; set; }
        public ICollection<VetService> VetServices { get; set; }
        = new List<VetService>();
        public int? ExperienceYears { get; set; }     
        public double? Price { get; set; }            
        public string? WorkingDays { get; set; }      
        public string? WorkingTime { get; set; }      
        public double? Latitude { get; set; }        
        public double? Longitude { get; set; }        
        public int? RewardPoints { get; set; }        
        public int? WaitingTimeMinutes { get; set; }  
        public string? ImageUrl { get; set; }

        // Optional: extra fields for searching/normalization
        public string? NameLower { get; set; }
        public string? FirstName { get; set; }
        public string? FirstNameLower { get; set; }
        public string? LastName { get; set; }
        public string? LastNameLower { get; set; }
    }
}
