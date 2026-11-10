using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModuloAdminBodegon.Models;

namespace ModuloAdminBodegon.Controllers
{
    public class ReservasController : Controller
    {
        private readonly BodegonContext _context;

        public ReservasController(BodegonContext context)
        {
            _context = context;
        }

        // GET: /Reservas/Index -> listado con filtro por fecha y búsqueda por número
        public IActionResult Index(DateTime? fechaFiltro, string? numeroReserva)
        {

            // Las reservas activas de días anteriores pasan a Completada -> Reserva.CompletarSiYaPaso()
            List<Reserva> vencidas = _context.Reservas
                .Where(r => r.Estado == EstadoReserva.Activa && r.Fecha.Date < DateTime.Today)
                .ToList();

            foreach (Reserva r in vencidas)
            {
                r.CompletarSiYaPaso();
            }
            _context.SaveChanges();

            // Armamos la consulta de a poco. Todavía no se ejecuta: se ejecuta en el ToList()
            IQueryable<Reserva> consulta = _context.Reservas.Include(r => r.Cliente);

            if (fechaFiltro != null)
            {
                consulta = consulta.Where(r => r.Fecha.Date == fechaFiltro.Value.Date);
            }

            if (!string.IsNullOrWhiteSpace(numeroReserva))
            {
                consulta = consulta.Where(r => r.NumeroReserva.Contains(numeroReserva.Trim()));
            }

            // Sin ningún filtro mostramos solo las activas.
            // Si se filtra, se muestran todas (también las canceladas)
            if (fechaFiltro == null && string.IsNullOrWhiteSpace(numeroReserva))
            {
                consulta = consulta.Where(r => r.Estado == EstadoReserva.Activa);
            }

            // Para que los filtros sigan escritos en la pantalla
            ViewBag.FechaFiltro = fechaFiltro?.ToString("yyyy-MM-dd");
            ViewBag.NumeroReserva = numeroReserva;

            List<Reserva> reservas = consulta.OrderByDescending(r => r.Fecha).ToList();
            return View("Reservas", reservas);
        }

        // GET: /Reservas/Create -> formulario de reserva presencial
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Reservas/Create -> graba la reserva presencial
        // ASP.NET convierte solo el texto del formulario a DateTime y a TipoTurno
        [HttpPost]
        public IActionResult Create(string Nombre, string Telefono, string Email, DateTime Fecha, TipoTurno Turno, int CantidadPersonas)
        {
            // Guardamos lo que se escribió para volver a mostrarlo si hay un error
            GuardarViewBag(Nombre, Telefono, Email, Fecha, Turno, CantidadPersonas);

            // 1. Validamos el cliente -> Cliente.TieneDatosValidos()
            Cliente datosCliente = new Cliente { Nombre = Nombre, Telefono = Telefono, Email = Email };
            if (!datosCliente.TieneDatosValidos())
            {
                ModelState.AddModelError("", "Los datos del cliente no son válidos (verifique nombre, teléfono y email).");
                return View();
            }

            // 2. Validamos la reserva -> Reserva.EsValida()
            Reserva reserva = new Reserva { Fecha = Fecha, Turno = Turno, CantidadPersonas = CantidadPersonas };
            if (!reserva.EsValida())
            {
                ModelState.AddModelError("", "La fecha debe ser actual/futura y no puede ser lunes (local cerrado). La cantidad de personas debe ser al menos 1.");
                return View();
            }

            // 3. Controlamos el cupo -> TurnoLibre.HayCupo()
            TurnoLibre? turnoLibre = _context.TurnosLibres.FirstOrDefault(t => t.Turno == Turno);
            if (turnoLibre == null)
            {
                ModelState.AddModelError("", "El turno elegido no está habilitado.");
                return View();
            }

            int personasOcupadas = ObtenerPersonasOcupadas(Fecha, Turno);
            if (!turnoLibre.HayCupo(CantidadPersonas, personasOcupadas))
            {
                ModelState.AddModelError("", "No hay suficientes cupos para este turno. Lugares disponibles: " + turnoLibre.CalcularLugaresLibres(personasOcupadas) + ".");
                return View();
            }

            // 4. Buscamos el cliente -> Cliente.EsMismoCliente(); si no existe, se usa el nuevo
            Cliente? existente = _context.Clientes.FirstOrDefault(c => c.Email == Email);
            if (existente != null && existente.EsMismoCliente(Nombre, Email, Telefono))
            {
                reserva.Cliente = existente;
            }
            else
            {
                reserva.Cliente = datosCliente;
            }

            // 5. Le asignamos el número a la reserva -> Reserva.AsignarNumero(), que usa SecuenciaReserva.GenerarNumeroReserva()
            SecuenciaReserva secuencia = _context.SecuenciasReserva.First(s => s.Id == 1);
            reserva.AsignarNumero(secuencia);

            // 6. Guardamos todo junto (cliente nuevo si hay, secuencia y reserva). Estado inicial: Activa
            _context.Reservas.Add(reserva);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // POST: /Reservas/Cancelar/5 -> borrado lógico: la reserva pasa a Cancelada
        [HttpPost]
        public IActionResult Cancelar(int id)
        {
            Reserva? reserva = _context.Reservas.Find(id);
            if (reserva != null)
            {
                reserva.Cancelar();        // método de la clase Reserva
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: /Reservas/GetDisponibilidad?fecha=... -> lo llama validaciones.js y devuelve JSON
        [HttpGet]
        public IActionResult GetDisponibilidad(DateTime fecha)
        {
            List<TurnoLibre> turnos = _context.TurnosLibres.ToList();
            List<object> resultado = new List<object>();

            foreach (TurnoLibre t in turnos)
            {
                int disponibles = t.CalcularLugaresLibres(ObtenerPersonasOcupadas(fecha, t.Turno));

                resultado.Add(new
                {
                    turno = t.Turno.ToString(),   // "Almuerzo", "Merienda", "Cena"
                    disponibles = disponibles,
                    completo = disponibles <= 0
                });
            }

            return Json(resultado);
        }

        // Consulta a la base: suma las personas de las reservas activas de ese día y turno
        private int ObtenerPersonasOcupadas(DateTime fecha, TipoTurno turno)
        {
            return _context.Reservas
                .Where(r => r.Fecha.Date == fecha.Date && r.Turno == turno && r.Estado == EstadoReserva.Activa)
                .Sum(r => r.CantidadPersonas);
        }

        // Conserva los datos en los inputs del formulario cuando hay un error
        private void GuardarViewBag(string nombre, string telefono, string email, DateTime fecha, TipoTurno turno, int cantidadPersonas)
        {
            ViewBag.Nombre = nombre;
            ViewBag.Telefono = telefono;
            ViewBag.Email = email;
            ViewBag.Fecha = fecha.ToString("yyyy-MM-dd");   // formato que entiende el input date
            ViewBag.Turno = turno.ToString();                // "Almuerzo", "Merienda" o "Cena"
            ViewBag.CantidadPersonas = cantidadPersonas > 0 ? cantidadPersonas : (int?)null;
        }
    }
}
