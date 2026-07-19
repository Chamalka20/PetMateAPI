using Microsoft.AspNetCore.Mvc;
using PetMateAPI.DTOs;
using PetMateAPI.Services;

namespace PetMateAPI.Controllers
{
    [ApiController]
    [Route("api/services")]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceService _serviceService;


        public ServicesController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }



        [HttpPost]
        public async Task<IActionResult> CreateService(
            [FromBody] CreateServiceDto dto)
        {
            try
            {
                var service =
                    await _serviceService.CreateService(dto);


                return Ok(new
                {
                    message = "Service added successfully",
                    data = service
                });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
