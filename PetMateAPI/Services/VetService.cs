using PetMateAPI.Data;
using PetMateAPI.Models;
using Microsoft.EntityFrameworkCore;


public interface IVetService
{
    Task<(List<Vet> Vets, int Total)> GetFilteredVets(VetFilterRequest filter);
    Task<Vet> GetVetById(int id);
    Task<int> BulkInsert(List<Vet> vets);
}

public class VetService : IVetService
{
    private readonly AppDbContext _context;

    public VetService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(List<Vet> Vets, int Total)> GetFilteredVets(VetFilterRequest filter)
    {
        // Start with full collection
        var query = _context.Vets.AsQueryable();

        // ── Search ────────────────────────────────────────────────────────────
        if (!string.IsNullOrWhiteSpace(filter.SearchQuery))
        {
            var q = filter.SearchQuery.ToLower().Trim();

            query = query.Where(v =>
                (v.Name != null && v.Name.ToLower().Contains(q)) ||
                (v.Services != null && v.Services.Any(s => s.ToLower().Contains(q)))
            );
        }

        // ── Services Filter ───────────────────────────────────────────────────
        if (filter.Services != null && filter.Services.Any())
        {
            query = query.Where(v =>
                v.Services != null &&
                filter.Services.All(s => v.Services.Contains(s))
            );
        }

        // ── Min Rating ────────────────────────────────────────────────────────
        if (filter.MinRating.HasValue && filter.MinRating > 0)
        {
            query = query.Where(v => v.Rating >= filter.MinRating.Value);
        }

        // ── Max Price ─────────────────────────────────────────────────────────
        if (filter.MaxPrice.HasValue && filter.MaxPrice < 5000)
        {
            query = query.Where(v => v.Price == null || v.Price <= filter.MaxPrice.Value);
        }

        // ── Max Waiting Time ──────────────────────────────────────────────────
        if (filter.MaxWaitingTime.HasValue && filter.MaxWaitingTime < 60)
        {
            query = query.Where(v => v.WaitingTimeMinutes == null ||
                                     v.WaitingTimeMinutes <= filter.MaxWaitingTime.Value);
        }

        // ── Sort ──────────────────────────────────────────────────────────────
        query = filter.SortBy switch
        {
            "RATING_HIGH" => query.OrderByDescending(v => v.Rating),
            "PRICE_LOW" => query.OrderBy(v => v.Price),
            "PRICE_HIGH" => query.OrderByDescending(v => v.Price),
            "WAITING_TIME" => query.OrderBy(v => v.WaitingTimeMinutes),
            _ => query.OrderBy(v => v.Id)   // default — no sort
        };

        // ── Pagination ────────────────────────────────────────────────────────
        var total = await query.CountAsync();
        var vets = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (vets, total);
    }


    public async Task<Vet> GetVetById(int id)
    {
        return await _context.Vets.FindAsync(id);
    }

    public async Task<int> BulkInsert(List<Vet> vets)
    {
        await _context.Vets.AddRangeAsync(vets);
        return await _context.SaveChangesAsync();
    }
}