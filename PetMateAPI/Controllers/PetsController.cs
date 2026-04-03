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

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreatePet(CreatePetDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var pet = await _petService.CreatePetAsync(dto, userId);

        return Ok(pet);
    }
}

