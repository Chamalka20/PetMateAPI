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
         
            pets = pets
        });
    }

    [HttpGet("{petId}")]
    public async Task<IActionResult> GetPet(int petId)
    {
        var pet = await _petService.GetPetAsync(GetUserId(), petId);
        if (pet == null)
            return NotFound(new { message = "Pet not found." });

        return Ok(pet);
    }

    // PUT /api/pets/5
    [HttpPut("{petId}")]
    public async Task<IActionResult> UpdatePet(
        int petId, [FromBody] UpdatePetDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var pet = await _petService.UpdatePetAsync(GetUserId(), petId, dto);
        if (pet == null)
            return NotFound(new { message = "Pet not found." });

        return Ok(new
        {
            message = "Pet updated successfully!",
            pet = pet
        });
    }

    // DELETE /api/pets/5
    [HttpDelete("{petId}")]
    public async Task<IActionResult> DeletePet(int petId)
    {
        var success = await _petService.DeletePetAsync(GetUserId(), petId);
        if (!success)
            return NotFound(new { message = "Pet not found." });

        return Ok(new { message = "Pet deleted successfully." });
    }
}

