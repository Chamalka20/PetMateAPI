using PetMateAPI.Data;
using PetMateAPI.Models;
using Microsoft.EntityFrameworkCore;


public interface IVetService
{
    Task<List<Vet>> GetVets(int page, int pageSize);
    Task<int> GetTotalCount();
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

    public async Task<List<Vet>> GetVets(int page, int pageSize)
    {
        return await _context.Vets
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCount()
    {
        return await _context.Vets.CountAsync();
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