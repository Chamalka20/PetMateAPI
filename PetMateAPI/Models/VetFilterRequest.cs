namespace PetMateAPI.Models
{
    public class VetFilterRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Search
        public string? SearchQuery { get; set; }

        // Filter
        public List<string>? Services { get; set; }
        public double? MinRating { get; set; }
        public double? MaxPrice { get; set; }
        public int? MaxWaitingTime { get; set; }

        // Sort
        public string? SortBy { get; set; } 
    }
}
