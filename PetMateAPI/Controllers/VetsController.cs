using Microsoft.AspNetCore.Mvc;
using PetMateAPI.Data;
using PetMateAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace PetMateAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VetsController : ControllerBase
    {
        private readonly IVetService _vetService;

        public VetsController(IVetService vetService)
        {
            _vetService = vetService;
        }

        [HttpGet]
        public async Task<IActionResult> GetVets(int page = 1, int pageSize = 10)
        {
            var vets = await _vetService.GetVets(page, pageSize);
            var total = await _vetService.GetTotalCount();

            return Ok(new
            {
                total,
                page,
                pageSize,
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
        public async Task<IActionResult> BulkInsert(List<Vet> vets)
        {
            if (vets == null || !vets.Any())
                return BadRequest("No data provided");

            var count = await _vetService.BulkInsert(vets);

            return Ok($"{count} vets inserted");
        }
    }
}
