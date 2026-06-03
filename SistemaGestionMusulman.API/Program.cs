using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SistemaGestionMusulman.API.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- CONFIGURACIÓN CORS (Permitir al Frontend conectarse) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

/* Add services to the container. */
builder.Services.AddControllers();

/* Le decimos a .NET que use nuestro puente y se conecte a PostgreSQL */
builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
    opciones.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

/* --- AQUÍ ENCENDEMOS SWAGGER --- */
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autorización JWT usando el esquema Bearer. \r\n\r\n Escribe la palabra 'Bearer' [espacio] y luego tu token en la caja de abajo.\r\n\r\nEjemplo: 'Bearer eyJhbGciOiJIUzI1NiIs...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

/* --- INYECCIÓN DE DEPENDENCIAS --- */
builder.Services.AddScoped<SistemaGestionMusulman.API.Repositories.IPerfilMusulmanRepository, SistemaGestionMusulman.API.Repositories.PerfilMusulmanRepository>();
builder.Services.AddScoped<SistemaGestionMusulman.API.Services.IPerfilMusulmanService, SistemaGestionMusulman.API.Services.PerfilMusulmanService>();

builder.Services.AddScoped<SistemaGestionMusulman.API.Repositories.ISadaqahRepository, SistemaGestionMusulman.API.Repositories.SadaqahRepository>();
builder.Services.AddScoped<SistemaGestionMusulman.API.Services.ISadaqahService, SistemaGestionMusulman.API.Services.SadaqahService>();

builder.Services.AddScoped<SistemaGestionMusulman.API.Repositories.IMadrasaRepository, SistemaGestionMusulman.API.Repositories.MadrasaRepository>();
builder.Services.AddScoped<SistemaGestionMusulman.API.Services.IMadrasaService, SistemaGestionMusulman.API.Services.MadrasaService>();

/* --- 1. ACTIVAR IDENTITY --- */
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

/* --- 2. CONFIGURAR LOS GUARDIAS JWT --- */
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

var app = builder.Build();

/* Configure the HTTP request pipeline. */
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --- ENCENDER CORS ---
app.UseCors("PermitirFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();