using CemSys3.Business.Login;
using CemSys3.Business.Usuario;
using CemSys3.Interfaces.Login;
using CemSys3.Interfaces.Usuario;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

//para el manejo de sesiones
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(60); // Tiempo de expiración por inactividad
});

// Add services to the container.
builder.Services.AddControllersWithViews();


// Configurar el DbContext para Entity Framework Core con SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Conexion")));


//Inyectar dependencias de servicios personalizados
builder.Services.AddScoped<ILogin, LoginService>();
builder.Services.AddScoped<IUsuario, UsuarioService>();

var app = builder.Build();
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}")
    .WithStaticAssets();


app.Run();
