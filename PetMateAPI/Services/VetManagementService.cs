using Microsoft.EntityFrameworkCore;
using PetMateAPI.Data;
using PetMateAPI.DTOs;
using PetMateAPI.Models;


public interface IVetManagementService
{
    Task<(List<VetDto> Vets, int Total)> GetFilteredVets(VetFilterRequest filter);
    Task<Vet> GetVetById(int id);
    Task<int> BulkInsert(List<VetImportDto> vets);
}

public class VetManagementService : IVetManagementService
{
    private readonly AppDbContext _context;

    public VetManagementService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(List<VetDto> Vets, int Total)> GetFilteredVets(VetFilterRequest filter)
    {
        var query = _context.Vets
            .Include(v => v.VetServices)
                .ThenInclude(vs => vs.Service)
            .AsQueryable();

        // ── Search ────────────────────────────────────────────────────────────
        if (!string.IsNullOrWhiteSpace(filter.SearchQuery))
        {
            var q = filter.SearchQuery.Trim();

            query = query.Where(v =>
                (v.Name != null && v.Name.Contains(q)) ||
                v.VetServices.Any(vs => vs.Service.Name.Contains(q))
            );
        }

        // ── Services Filter ───────────────────────────────────────────────────
        if (filter.Services != null && filter.Services.Any())
        {
            query = query.Where(v =>
                filter.Services.All(service =>
                    v.VetServices.Any(vs => vs.Service.Name == service)
                )
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
            query = query.Where(v =>
                v.Price == null || v.Price <= filter.MaxPrice.Value);
        }

        // ── Max Waiting Time ──────────────────────────────────────────────────
        if (filter.MaxWaitingTime.HasValue && filter.MaxWaitingTime < 60)
        {
            query = query.Where(v =>
                v.WaitingTimeMinutes == null ||
                v.WaitingTimeMinutes <= filter.MaxWaitingTime.Value);
        }

        // ── Sort ──────────────────────────────────────────────────────────────
        query = filter.SortBy switch
        {
            "RATING_HIGH" => query.OrderByDescending(v => v.Rating),
            "PRICE_LOW" => query.OrderBy(v => v.Price),
            "PRICE_HIGH" => query.OrderByDescending(v => v.Price),
            "WAITING_TIME" => query.OrderBy(v => v.WaitingTimeMinutes),
            _ => query.OrderBy(v => v.Id)
        };

        // ── Pagination ────────────────────────────────────────────────────────
        var total = await query.CountAsync();

        var vets = await query
         .Skip((filter.Page - 1) * filter.PageSize)
         .Take(filter.PageSize)
         .Select(v => new VetDto
         {
             Id = v.Id,
             Name = v.Name,
             ClinicName = v.ClinicName,
             Location = v.Location,
             Rating = v.Rating,
             ExperienceYears = v.ExperienceYears,
             Price = v.Price,
             WorkingDays = v.WorkingDays,
             WorkingTime = v.WorkingTime,
             Latitude = v.Latitude,
             Longitude = v.Longitude,
             RewardPoints = v.RewardPoints,
             WaitingTimeMinutes = v.WaitingTimeMinutes,
             ImageUrl = v.ImageUrl,

             Services = v.VetServices
                 .Select(vs => vs.Service.Name)
                 .ToList()
         })
         .ToListAsync();

        return (vets, total);
    }


    public async Task<Vet> GetVetById(int id)
    {
        return await _context.Vets.FindAsync(id);
    }

    public async Task<int> BulkInsert(List<VetImportDto> vets)
    {
        var vetEntities = vets.Select(dto => new Vet
        {
            Name = dto.Name,
            ClinicName = dto.ClinicName,
            Location = dto.Location,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Rating = dto.Rating,
            Specializations = dto.Specializations,
            ExperienceYears = dto.ExperienceYears,
            Price = dto.Price,
            WorkingDays = dto.WorkingDays,
            WorkingTime = dto.WorkingTime,
            RewardPoints = dto.RewardPoints,
            WaitingTimeMinutes = dto.WaitingTimeMinutes,
            ImageUrl = dto.ImageUrl,
            NameLower = dto.Name?.ToLower(),
            FirstName = dto.Name?.Split(' ').FirstOrDefault(),
            FirstNameLower = dto.Name?.Split(' ').FirstOrDefault()?.ToLower(),
            LastName = dto.Name != null && dto.Name.Split(' ').Length > 1
                ? string.Join(" ", dto.Name.Split(' ').Skip(1))
                : null,
            LastNameLower = dto.Name != null && dto.Name.Split(' ').Length > 1
                ? string.Join(" ", dto.Name.Split(' ').Skip(1)).ToLower()
                : null
        }).ToList();

        _context.Vets.AddRange(vetEntities);
        await _context.SaveChangesAsync();   

        var vetServices = new List<VetService>();
        for (int i = 0; i < vetEntities.Count; i++)
        {
            foreach (var serviceId in vets[i].ServiceIds)
            {
                vetServices.Add(new VetService
                {
                    VetId = vetEntities[i].Id,
                    ServiceId = serviceId
                });
            }
        }

        _context.VetServices.AddRange(vetServices);
        await _context.SaveChangesAsync();

        return vetEntities.Count;   
    }
}