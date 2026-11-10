using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using ModuloAdminBodegon.Models;

var builder = WebApplication.CreateBuilder(args);

// MVC: controladores + vistas
builder.Services.AddControllersWithViews();

// Base de datos (es la misma que usa el Módulo Usuario)
builder.Services.AddDbContext<BodegonContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BodegonConnection")));

var app = builder.Build();

// Todas las URLs del módulo empiezan con /ModuloAdminBodegon
app.UsePathBase("/ModuloAdminBodegon");

// Fotos de los platos: carpeta compartida con el Módulo Usuario (ruta en appsettings.json)
string rutaImagenes = builder.Configuration["RutaImagenes"]!;
Directory.CreateDirectory(rutaImagenes);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(rutaImagenes),
    RequestPath = "/imagenes"
});

// Archivos de wwwroot (css, js, logo)
app.UseStaticFiles();

app.UseRouting();

// Ruta por defecto: /Controlador/Accion/id
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();