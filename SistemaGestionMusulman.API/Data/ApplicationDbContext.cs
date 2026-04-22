using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaGestionMusulman.API.Models;

namespace SistemaGestionMusulman.API.Data
{
    // 👇 ¡NUEVO! Cambiamos DbContext por IdentityDbContext para activar la seguridad
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Tus tablas existentes (No tocamos nada aquí)
        public DbSet<PerfilMusulman> PerfilesMusulmanes { get; set; }
        public DbSet<DonacionSadaqah> DonacionesSadaqah { get; set; }
        public DbSet<ClaseMadrasa> ClasesMadrasa { get; set; }
        public DbSet<InscripcionClase> InscripcionesClases { get; set; }

        // 👇 ¡NUEVO! Es obligatorio agregar esto para que Identity cree sus tablas sin error
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}