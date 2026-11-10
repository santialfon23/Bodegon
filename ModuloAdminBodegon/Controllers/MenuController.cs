using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ModuloAdminBodegon.Models;

namespace ModuloAdminBodegon.Controllers
{
    public class MenuController : Controller
    {
        private readonly BodegonContext _context;
        private readonly string _rutaImagenes;

        public MenuController(BodegonContext context, IConfiguration configuration)
        {
            _context = context;
            // Carpeta compartida de imágenes (definida en appsettings.json)
            _rutaImagenes = configuration["RutaImagenes"]!;
        }

        // GET: /Menu/Index -> categorías con sus platos, ordenadas por posición
        public IActionResult Index()
        {
            List<Categoria> categorias = _context.Categorias
                .Include(c => c.Platos)
                .OrderBy(c => c.Posicion)
                .ToList();

            return View(categorias);
        }

        // ==========================================
        // PLATOS
        // ==========================================

        // GET: /Menu/Crear -> formulario vacío
        [HttpGet]
        public IActionResult Crear()
        {
            CargarCategorias();
            return View();
        }

        // POST: /Menu/Crear -> graba el plato nuevo
        [HttpPost]
        public IActionResult Crear(Plato plato, IFormFile? imagenFile)
        {
            // 1. Validamos el plato -> Plato.TieneDatosValidos()
            if (!plato.TieneDatosValidos())
            {
                ModelState.AddModelError("", "Verificá los datos: nombre y descripción obligatorios, precio mayor a cero.");
                CargarCategorias();
                return View(plato);
            }

            // 2. Buscamos la categoría elegida
            Categoria? categoria = _context.Categorias.Find(plato.CategoriaId);
            if (categoria == null)
            {
                ModelState.AddModelError("", "La categoría seleccionada no existe.");
                CargarCategorias();
                return View(plato);
            }

            // 3. Guardamos la imagen en la carpeta compartida
            plato.ImagenUrl = GuardarImagen(imagenFile);

            // 4. Agregamos el plato a su categoría -> Categoria.AgregarPlato() y grabamos
            categoria.AgregarPlato(plato);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: /Menu/Editar/5 -> formulario con los datos actuales del plato
        [HttpGet]
        public IActionResult Editar(int id)
        {
            Plato? plato = _context.Platos.Find(id);
            if (plato == null)
            {
                return NotFound();
            }

            CargarCategorias();
            return View(plato);
        }

        // POST: /Menu/Editar/5 -> graba los cambios del plato
        [HttpPost]
        public IActionResult Editar(Plato plato, IFormFile? imagenFile)
        {
            // 1. Validamos el plato -> Plato.TieneDatosValidos()
            if (!plato.TieneDatosValidos())
            {
                ModelState.AddModelError("", "Verificá los datos: nombre y descripción obligatorios, precio mayor a cero.");
                CargarCategorias();
                return View(plato);
            }

            // 2. Si subieron una imagen nueva la guardamos.
            //    Si no, queda la anterior (viene en el campo oculto ImagenUrl del formulario)
            string imagenNueva = GuardarImagen(imagenFile);
            if (imagenNueva != "")
            {
                plato.ImagenUrl = imagenNueva;
            }

            // 3. Update marca el plato como modificado y SaveChanges lo graba
            _context.Platos.Update(plato);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: /Menu/Eliminar/5 -> borra el plato
        public IActionResult Eliminar(int id)
        {
            Plato? plato = _context.Platos.Find(id);
            if (plato != null)
            {
                _context.Platos.Remove(plato);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // ==========================================
        // CATEGORÍAS
        // ==========================================

        // GET: /Menu/CrearCategoria -> formulario vacío
        [HttpGet]
        public IActionResult CrearCategoria()
        {
            return View();
        }

        // POST: /Menu/CrearCategoria -> graba la categoría nueva
        [HttpPost]
        public IActionResult CrearCategoria(Categoria categoria)
        {
            // Validamos la categoría -> Categoria.TieneDatosValidos()
            if (!categoria.TieneDatosValidos())
            {
                ModelState.AddModelError("", "Verificá los datos: nombre obligatorio y posición mayor a cero.");
                return View(categoria);
            }

            _context.Categorias.Add(categoria);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: /Menu/EditarCategoria/5 -> formulario con los datos actuales
        [HttpGet]
        public IActionResult EditarCategoria(int id)
        {
            Categoria? categoria = _context.Categorias.Find(id);
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }

        // POST: /Menu/EditarCategoria/5 -> graba los cambios
        [HttpPost]
        public IActionResult EditarCategoria(Categoria categoria)
        {
            // Validamos la categoría -> Categoria.TieneDatosValidos()
            if (!categoria.TieneDatosValidos())
            {
                ModelState.AddModelError("", "Verificá los datos: nombre obligatorio y posición mayor a cero.");
                return View(categoria);
            }

            _context.Categorias.Update(categoria);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: /Menu/EliminarCategoria/5 -> borra la categoría y sus platos
        public IActionResult EliminarCategoria(int id)
        {
            // Include trae los platos, así EF los borra junto con la categoría
            Categoria? categoria = _context.Categorias
                .Include(c => c.Platos)
                .FirstOrDefault(c => c.Id == id);

            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // ==========================================
        // MÉTODOS PRIVADOS (ayudan a las acciones)
        // ==========================================

        // Carga la lista de categorías para el desplegable del formulario de platos
        private void CargarCategorias()
        {
            ViewBag.Categorias = new SelectList(_context.Categorias.OrderBy(c => c.Posicion).ToList(), "Id", "Nombre");
        }

        // Guarda el archivo en la carpeta compartida con un nombre único y devuelve ese nombre.
        // Si no se subió ningún archivo, devuelve "" (texto vacío)
        private string GuardarImagen(IFormFile? imagenFile)
        {
            if (imagenFile == null || imagenFile.Length == 0)
            {
                return "";
            }

            // Nombre único (GUID) + la extensión original (.jpg, .png...)
            string nombreUnico = Guid.NewGuid().ToString() + Path.GetExtension(imagenFile.FileName);
            string rutaCompleta = Path.Combine(_rutaImagenes, nombreUnico);

            // Copiamos el archivo subido al disco
            using (FileStream stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                imagenFile.CopyTo(stream);
            }

            return nombreUnico;
        }
    }
}
