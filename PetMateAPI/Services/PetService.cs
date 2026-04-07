using Microsoft.EntityFrameworkCore;
using PetMateAPI.Data;
using PetMateAPI.DTOs;
using PetMateAPI.Models;

namespace PetMateAPI.Services;

public interface IPetService
{
    Task<Pet> CreatePetAsync(PetDto dto, string userId);
    Task<List<PetResponseDto>> GetPetListAsync(string userId);
}
public class PetService : IPetService
{
    private readonly AppDbContext _context;

    public PetService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Pet> CreatePetAsync(PetDto dto, string userId)
    {
        var pet = new Pet
        {
            Name = dto.Name,
            Type = dto.Type,
            Breed = dto.Breed,
            Age = dto.Age,
            Weight = dto.Weight,
            Gender = dto.Gender,
            IsSpayedNeutered = dto.IsSpayedNeutered,
            MedicalConditions = string.Join(",", dto.MedicalConditions),
            Allergies = string.Join(",", dto.Allergies),
            ImageUrl = dto.ImageUrl,
            UserId = userId
        };

        _context.Pets.Add(pet);
        await _context.SaveChangesAsync();

        return pet;
    }

    public async Task<List<PetResponseDto>> GetPetListAsync(string userId)
    {
        var pets = await _context.Pets
            .Include(p => p.User)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return pets.Select(p => MapToDto(p)).ToList();
    }

    private PetResponseDto MapToDto(Pet pet) => new()
    {
        Id = pet.Id,
        Name = pet.Name,
        Type = pet.Type,
        Breed = pet.Breed,
        Age = pet.Age,
        Gender = pet.Gender,
        PhotoUrl = pet.ImageUrl,
        UserId = pet.UserId,
        OwnerName = pet.User?.FullName ?? "",
        CreatedAt = pet.CreatedAt
    };

    private async Task<PetResponseDto> ToDto(Pet pet)
    {
        await _context.Entry(pet).Reference(p => p.User).LoadAsync();
        return MapToDto(pet);
    }

}
