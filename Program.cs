using CemSys3.Business.Archivo;
using CemSys3.Business.Cementerio;
using CemSys3.Business.ConceptoTarifaria;
using CemSys3.Business.Concesion;
using CemSys3.Business.EmpresaSepelio;
using CemSys3.Business.HistorialEstadoService;
using CemSys3.Business.Ingreso;
using CemSys3.Business.Login;
using CemSys3.Business.Notas;
using CemSys3.Business.Notificacion;
using CemSys3.Business.Parcela;
using CemSys3.Business.PDF;
using CemSys3.Business.Persona;
using CemSys3.Business.PlantillaTramite;
using CemSys3.Business.Seccion;
using CemSys3.Business.Tarea;
using CemSys3.Business.Tarifaria;
using CemSys3.Business.Tramite;
using CemSys3.Business.TramiteConcesion;
using CemSys3.Business.Usuario;
using CemSys3.Helpers.PDF;
using CemSys3.Interfaces.Archivo;
using CemSys3.Interfaces.Cementerio;
using CemSys3.Interfaces.ConceptoTarifaria;
using CemSys3.Interfaces.Concesion;
using CemSys3.Interfaces.EmpresaSepelio;
using CemSys3.Interfaces.HistorialEstados;
using CemSys3.Interfaces.Ingreso;
using CemSys3.Interfaces.Login;
using CemSys3.Interfaces.Notas;
using CemSys3.Interfaces.Notificaciones;
using CemSys3.Interfaces.Parcela;
using CemSys3.Interfaces.PDF;
using CemSys3.Interfaces.Persona;
using CemSys3.Interfaces.PlantillaTramite;
using CemSys3.Interfaces.Seccion;
using CemSys3.Interfaces.Tarea;
using CemSys3.Interfaces.Tarifaria;
using CemSys3.Interfaces.Tramite;
using CemSys3.Interfaces.TramitesConcesion;
using CemSys3.Interfaces.Usuario;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;
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
builder.Services.AddScoped<IEmpresaSepelio, EmpresaSepelioService>();
builder.Services.AddScoped<ICementerio, CementerioService>();
builder.Services.AddScoped<ISeccion, SeccionService>();
builder.Services.AddScoped<ISeccionNichoTarifaria, SeccionService>();
builder.Services.AddScoped<IParcela, ParcelaService>();
builder.Services.AddScoped<IConceptoTarifaria, ConceptoTarifariaService>();
builder.Services.AddScoped<ITarifaria, TarifariaService>();
builder.Services.AddScoped<IPrecioIngresoService, PrecioIngresoService>();
builder.Services.AddScoped<INotas, NotaService>();
builder.Services.AddScoped<ITarea, TareaService>();
builder.Services.AddScoped<IHistorialEstados, HistorialEstadoService>();
builder.Services.AddScoped<ITramite, TramiteService>();
builder.Services.AddScoped<IIngreso, IngresoService>();
builder.Services.AddScoped<IPersona, PersonaService>();
builder.Services.AddScoped<IArchivo, ArchivoService>();
builder.Services.AddScoped<IConcesion, ConcesionService>();
builder.Services.AddScoped<INotificaciones, NotificacionService>();
builder.Services.AddScoped<IViewRenderService, ViewRenderService>();
builder.Services.AddScoped<IDeudaConcesion, DeudaConcesionService>();
builder.Services.AddScoped<IPlantillaTramite, PlantillaTramiteService>();
builder.Services.AddScoped<IDocumentoTramiteService, DocumentoTramiteService>();
builder.Services.AddScoped<ITemplateProcessor, TemplateProcessor>();
builder.Services.AddScoped<ICancelarTramite, CancelarService>();
builder.Services.AddScoped<ITareaPlantilla, TareaPlantillaService>();
builder.Services.AddScoped<IRequisitos, RequisitosService>();

// =========================
// STRATEGIES
// =========================
//cambio de titular
builder.Services.AddScoped<CambioTitularStrategy>();
builder.Services.AddScoped<AceptacionTitularStrategy>();
builder.Services.AddScoped<CremacionStrategy>();


// =========================
// FACTORY
// =========================
builder.Services.AddScoped<IStrategyFactory, StrategyFactory>();



builder.Services.AddSingleton<IBrowser>(sp =>
{
    var playwright = Playwright.CreateAsync().GetAwaiter().GetResult();

    return playwright.Chromium
        .LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        })
        .GetAwaiter()
        .GetResult();
});

builder.Services.AddScoped<PlaywrightPdfGenerator>();

var app = builder.Build();
app.UseSession();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Calienta EF + SQL
    context.Concesiones.AsNoTracking().FirstOrDefault();

    // Opcional: precargar relaciones típicas
    context.Parcelas.AsNoTracking().FirstOrDefault();
    context.Personas.AsNoTracking().FirstOrDefault();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage(); // stacktrace solo en dev
}
app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}")
    .WithStaticAssets();


app.Run();


