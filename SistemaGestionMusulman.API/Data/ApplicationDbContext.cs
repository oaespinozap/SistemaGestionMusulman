using Microsoft.EntityFrameworkCore;
using SistemaGestionMusulman.API.Models;

namespace SistemaGestionMusulman.API.Data
{
    // Al heredar de "DbContext", le decimos a .NET que esta clase es nuestro puente hacia PostgreSQL
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Aquí le decimos qué "moldes" van a cruzar por el puente para convertirse en tablas
        public DbSet<PerfilMusulman> PerfilesMusulmanes { get; set; }

        // El módulo de inventario Sadaqah 
        public DbSet<DonacionSadaqah> DonacionesSadaqah { get; set; }

        // 👇 ¡NUEVO! Registramos nuestro Módulo de Educación (Madrasa)
        public DbSet<ClaseMadrasa> ClasesMadrasa { get; set; }
        public DbSet<InscripcionClase> InscripcionesClases { get; set; }
    }
}