namespace PetMateAPI.Models
{
    public class VetService
    {
        public int Id { get; set; }


        public int VetId { get; set; }

        public Vet Vet { get; set; } = null!;


        public int ServiceId { get; set; }

        public Service Service { get; set; } = null!;
    }
}
