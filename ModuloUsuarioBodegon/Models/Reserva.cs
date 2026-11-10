using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModuloUsuarioBodegon.Models
{

    public class Reserva
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int Id { get; set; }
        public string NumeroReserva { get; set; }
        public DateTime Fecha { get; set; }
        public int CantidadPersonas { get; set; }

        // Llave foránea y relación con el Cliente
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        // Relación con el Enum TipoTurno (Almuerzo, Merienda, Cena)
        public TipoTurno Turno { get; set; }


        public bool EsValida()
        {
            return EsFechaValida() && EsCantidadPersonasValida();
        }

        private bool EsFechaValida()
        {
            // La fecha debe ser hoy o futura y no puede ser lunes (local cerrado)
            return Fecha.Date >= DateTime.Today && Fecha.DayOfWeek != DayOfWeek.Monday;
        }

        private bool EsCantidadPersonasValida()
        {
            // Al menos 1 persona. El máximo lo controla el cupo del turno (TurnoLibre, en la base)
            return CantidadPersonas > 0;
        }

        public EstadoReserva Estado { get; set; } = EstadoReserva.Activa;


        // Recibe la secuencia, le pide el número siguiente y lo guarda como número de esta reserva
        public void AsignarNumero(SecuenciaReserva secuencia)
        {
            NumeroReserva = secuencia.GenerarNumeroReserva();
        }


    }




}