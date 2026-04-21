using Microsoft.EntityFrameworkCore;
using SistemaGestionMusulman.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Le decimos a .NET que use nuestro puente y se conecte a PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
    opciones.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- AQUÍ ENCENDEMOS SWAGGER ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// --- CONTRATACIÓN DE NUEVO PERSONAL (Inyección de Dependencias) ---
builder.Services.AddScoped<SistemaGestionMusulman.API.Repositories.IPerfilMusulmanRepository, SistemaGestionMusulman.API.Repositories.PerfilMusulmanRepository>();
builder.Services.AddScoped<SistemaGestionMusulman.API.Services.IPerfilMusulmanService, SistemaGestionMusulman.API.Services.PerfilMusulmanService>();
// --- CONTRATACIÓN DEL EQUIPO DE SADAQAH ---
builder.Services.AddScoped<SistemaGestionMusulman.API.Repositories.ISadaqahRepository, SistemaGestionMusulman.API.Repositories.SadaqahRepository>();
builder.Services.AddScoped<SistemaGestionMusulman.API.Services.ISadaqahService, SistemaGestionMusulman.API.Services.SadaqahService>();
// ------------------------------------------------------------------

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // ¡Esta es la famosa interfaz verde!
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();