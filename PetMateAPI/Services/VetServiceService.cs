using Microsoft.EntityFrameworkCore;
using PetMateAPI.Data;
using PetMateAPI.DTOs;
using PetMateAPI.Models;

namespace PetMateAPI.Services
{
    public interface IServiceService
    {
        Task<Service> CreateService(CreateServiceDto dto);

        Task<List<Service>> GetServices();
    }

    public class ServiceService : IServiceService
    {
        private readonly AppDbContext _context;


        public ServiceService(AppDbContext context)
        {
            _context = context;
        }


        public async Task<Service> CreateService(CreateServiceDto dto)
        {
            var exists = await _context.Services
                .AnyAsync(s => s.Name == dto.Name);


            if (exists)
            {
                throw new Exception("Service already exists");
            }


            var service = new Service
            {
                Name = dto.Name,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };


            _context.Services.Add(service);

            await _context.SaveChangesAsync();


            return service;
        }


        public async Task<List<Service>> GetServices()
        {
            return await _context.Services
                .Where(s => s.IsActive)
                .ToListAsync();
        }
    }
}
