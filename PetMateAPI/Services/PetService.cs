using PetMateAPI.Data;
using PetMateAPI.DTOs;
using PetMateAPI.Models;

namespace PetMateAPI.Services;

public interface IPetService
{
    Task<Pet> CreatePetAsync(CreatePetDto dto, string userId);
}
public class PetService : IPetService
{
    private readonly AppDbContext _context;

    public PetService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Pet> CreatePetAsync(CreatePetDto dto, string userId)
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
}
