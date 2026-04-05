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
        private readonly AppDbContext _context;

        public VetsController(AppDbContext context)
        {
            _context = context;
        }

        //  BULK INSERT
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkInsert(List<Vet> vets)
        {
            if (vets == null || !vets.Any())
                return BadRequest("No data provided");

            await _context.Vets.AddRangeAsync(vets);
            await _context.SaveChangesAsync();

            return Ok($"{vets.Count} vets inserted");
        }

        //  GET ALL VETS 
        [HttpGet]
        public async Task<IActionResult> GetVets()
        {
            var vets = await _context.Vets.ToListAsync();
            return Ok(vets);
        }
    }
}
