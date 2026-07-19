using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetMateAPI.Data;
using PetMateAPI.DTOs;
using PetMateAPI.Models;

namespace PetMateAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VetsController : ControllerBase
    {
        private readonly IVetManagementService _vetService;

        public VetsController(IVetManagementService vetService)
        {
            _vetService = vetService;
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetVets([FromQuery] VetFilterRequest filter)
        {
            var (vets, total) = await _vetService.GetFilteredVets(filter);
            return Ok(new
            {
                total,
                page = filter.Page,
                pageSize = filter.PageSize,
                data = vets
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVet(int id)
        {
            var vet = await _vetService.GetVetById(id);

            if (vet == null)
                return NotFound();

            return Ok(vet);
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> BulkInsert(List<VetImportDto> vets)
        {
            if (vets == null || !vets.Any())
                return BadRequest("No data provided");

            var count = await _vetService.BulkInsert(vets);

            return Ok($"{count} vets inserted");
        }
    }
}
