namespace PetMateAPI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetMateAPI.DTOs;
using PetMateAPI.Services;
using Supabase.Gotrue;
using System.Security.Claims;

[ApiController]
[Route("api/pets")]
public class PetsController : ControllerBase
{
    private readonly IPetService _petService;

    public PetsController(IPetService petService)
    {
        _petService = petService;
    }

    private string GetUserId() =>
       User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> CreatePet(PetDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var pet = await _petService.CreatePetAsync(dto, userId);

        return Ok(pet);
    }

    [Authorize]
    [HttpGet("list")]
    public async Task<IActionResult> GetPetList()
    {
        var pets = await _petService.GetPetListAsync(GetUserId());
        return Ok(new
        {
            count = pets.Count,
            pets = pets
        });
    }
}

