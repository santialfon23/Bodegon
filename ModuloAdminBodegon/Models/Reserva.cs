namespace ModuloAdminBodegon.Models
{
    public class Reserva
    {
        public int Id { get; set; }
        public string NumeroReserva { get; set; }
        public DateTime Fecha { get; set; }
        public int CantidadPersonas { get; set; }
        public TipoTurno Turno { get; set; }

        // Toda reserva nueva nace Activa. Al cancelarla no se borra: cambia de estado (borrado lógico)
        public EstadoReserva Estado { get; set; } = EstadoReserva.Activa;

        // Relación con el Cliente
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        public bool EsValida()
        {
            return EsFechaValida() && EsCantidadPersonasValida();
        }

        // La fecha debe ser hoy o futura y no puede ser lunes (local cerrado)
        private bool EsFechaValida()
        {
            return Fecha.Date >= DateTime.Today && Fecha.DayOfWeek != DayOfWeek.Monday;
        }

   

        // Recibe la secuencia, le pide el número siguiente y lo guarda como número de esta reserva
        public void AsignarNumero(SecuenciaReserva secuencia)
        {
            NumeroReserva = secuencia.GenerarNumeroReserva();
        }



    }
}
