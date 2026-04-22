using Microsoft.EntityFrameworkCore;
using SistemaGestionMusulman.API.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
// --- CONTRATACIÓN DEL EQUIPO DE MADRASA (EDUCACIÓN) ---
builder.Services.AddScoped<SistemaGestionMusulman.API.Repositories.IMadrasaRepository, SistemaGestionMusulman.API.Repositories.MadrasaRepository>();
builder.Services.AddScoped<SistemaGestionMusulman.API.Services.IMadrasaService, SistemaGestionMusulman.API.Services.MadrasaService>();

// ------------------------------------------------------------------
// --- 1. ACTIVAR IDENTITY (USUARIOS Y ROLES) ---
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// --- 2. CONFIGURAR LOS GUARDIAS JWT (PULSERAS VIP) ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // ¡Esta es la famosa interfaz verde!
}

app.UseHttpsRedirection();


app.UseAuthentication(); // <-- 1. Identifica QUIÉN eres (NUEVO)
app.UseAuthorization();  // <-- 2. Revisa QUÉ PUEDES hacer (Ya estaba)
app.MapControllers();
app.Run();