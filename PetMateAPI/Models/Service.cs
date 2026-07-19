namespace PetMateAPI.Models
{
    public class Service
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;


        public ICollection<VetService> VetServices { get; set; }
            = new List<VetService>();
    }
}
