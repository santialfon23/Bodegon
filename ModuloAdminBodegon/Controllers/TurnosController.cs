using Microsoft.AspNetCore.Mvc;
using ModuloAdminBodegon.Models;

namespace ModuloAdminBodegon.Controllers
{
    public class TurnosController : Controller
    {
        private readonly BodegonContext _context;

        public TurnosController(BodegonContext context)
        {
            _context = context;
        }

        // GET: /Turnos/Index -> tabla con la capacidad actual de cada turno
        public IActionResult Index()
        {
            List<TurnoLibre> turnos = _context.TurnosLibres
                .OrderBy(t => t.Id)
                .ToList();

            return View(turnos);
        }

        // GET: /Turnos/Editar/5 -> formulario con la capacidad actual del turno
        [HttpGet]
        public IActionResult Editar(int id)
        {
            TurnoLibre? turno = _context.TurnosLibres.Find(id);
            if (turno == null)
            {
                return NotFound();
            }

            return View(turno);
        }

        // POST: /Turnos/Editar/5 -> graba la nueva capacidad
        [HttpPost]
        public IActionResult Editar(int id, int capacidadMaxima)
        {
            // 1. Buscamos el turno en la base
            TurnoLibre? turno = _context.TurnosLibres.Find(id);
            if (turno == null)
            {
                return NotFound();
            }

            // 2. Validamos y actualizamos -> TurnoLibre.ActualizarCapacidad()
            if (!turno.ActualizarCapacidad(capacidadMaxima))
            {
                ModelState.AddModelError("", "La capacidad debe ser un número mayor a cero.");
                return View(turno);
            }

            // 3. Grabamos el cambio
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
