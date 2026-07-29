using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetMateAPI.Models;

namespace PetMateAPI.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Pet> Pets { get; set; }

    public DbSet<Vet> Vets { get; set; }

    public DbSet<Appointment> Appointments { get; set; }

    public DbSet<Service> Services { get; set; }

    public DbSet<VetService> VetServices { get; set; }

    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Medicine> Medicines { get; set; }




}