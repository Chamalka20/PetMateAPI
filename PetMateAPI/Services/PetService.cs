using Microsoft.EntityFrameworkCore;
using PetMateAPI.Data;
using PetMateAPI.DTOs;
using PetMateAPI.Models;

namespace PetMateAPI.Services;

public interface IPetService
{
    Task<Pet> CreatePetAsync(PetDto dto, string userId);
    Task<List<PetResponseDto>> GetPetListAsync(string userId);
    Task<PetResponseDto?> GetPetAsync(string userId, int petId);
    Task<PetResponseDto?> UpdatePetAsync(
      string userId, int petId, UpdatePetDto dto);
    Task<bool> DeletePetAsync(string userId, int petId);
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

    // ── Get single pet ────────────────────────────
    public async Task<PetResponseDto?> GetPetAsync(string userId, int petId)
    {
        var pet = await _context.Pets
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == petId && p.UserId == userId);

        return pet == null ? null : MapToDto(pet);
    }

    // ── Update ────────────────────────────────────
    public async Task<PetResponseDto?> UpdatePetAsync(
      string userId, int petId, UpdatePetDto dto)
    {
        var pet = await _context.Pets
            .FirstOrDefaultAsync(p => p.Id == petId && p.UserId == userId);

        if (pet == null) return null;

        if (dto.Name != null) pet.Name = dto.Name;
        if (dto.Type != null) pet.Type = dto.Type;
        if (dto.Breed != null) pet.Breed = dto.Breed;
        if (dto.Age.HasValue) pet.Age = dto.Age.Value;
        if (dto.Weight.HasValue) pet.Weight = dto.Weight.Value;
        if (dto.Gender != null) pet.Gender = dto.Gender;
        if (dto.IsSpayedNeutered.HasValue) pet.IsSpayedNeutered = dto.IsSpayedNeutered.Value;
        if (dto.MedicalConditions != null) pet.MedicalConditions = dto.MedicalConditions;
        if (dto.Allergies != null) pet.Allergies = dto.Allergies;
        if (dto.ImageUrl != null) pet.ImageUrl = dto.ImageUrl;

        await _context.SaveChangesAsync();

        return MapToDto(pet);
    }

    // ── Delete ────────────────────────────────────
    public async Task<bool> DeletePetAsync(string userId, int petId)
    {
        var pet = await _context.Pets
            .FirstOrDefaultAsync(p => p.Id == petId && p.UserId == userId);

        if (pet == null) return false;

        _context.Pets.Remove(pet);
        await _context.SaveChangesAsync();
        return true;
    }

    private PetResponseDto MapToDto(Pet pet) => new()
    {
        Id = pet.Id,
        Name = pet.Name,
        Type = pet.Type,
        Breed = pet.Breed,
        Age = pet.Age,
        Gender = pet.Gender,
        ImageUrl = pet.ImageUrl,
        UserId = pet.UserId,
        CreatedAt = pet.CreatedAt
    };

    private async Task<PetResponseDto> ToDto(Pet pet)
    {
        await _context.Entry(pet).Reference(p => p.User).LoadAsync();
        return MapToDto(pet);
    }

}
