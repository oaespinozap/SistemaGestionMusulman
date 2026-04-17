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